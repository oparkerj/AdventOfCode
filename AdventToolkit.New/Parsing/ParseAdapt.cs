using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Disambiguation;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// This class implements the algorithm used to adapt one type to another
/// in the parsing library.
///
/// The algorithm will try to convert one type to another in the following order:
/// - Direct assignment (no conversion)
/// - Using adapter from context
/// ~ Tuple to tuple conversion:
///   - If going larger to smaller, try constructing each target element
///   - Trim input to same size as output
///   - Trim input and adapt element-wise
/// - Adapt tuple to target via construction
/// - Unwrap 1-tuple and adapt to target
/// - Adapt tuple to target by adapting first element
/// - Adapt input to tuple via unpacking
/// ~ Enumerable conversion:
///   - If target is same as element type, just call .First()
///   - If target is IEnumerable, adapt element type
///   ~ Try to collect the target type
///     - Adapt sequence type to container type
///     - Adapt enumerable to container of tuple
///     - Try to construct the inner type
///   - Create target via construction
///   ~ Adapt enumerable to tuple
///     - Try to recurse into inner tuple
///     - Adapt element to tuple component
///     - Construct tuple component
///
/// Plans/Possibilities:
/// - Ability to have one collectable in the Enumerable to tuple conversion
/// </summary>
public static class ParseAdapt
{
    /// <summary>
    /// Adapt a parser to a specific output type.
    /// </summary>
    /// <param name="parser">Current parser.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <returns></returns>
    public static IParser Adapt(IParser parser, Type target, IParseContext context)
    {
        var output = ParseUtil.GetParserTypesOf(parser).OutputType;
        Parse.Verbose($"Adapting parser {parser.GetType()} ({output}) to {target}");
        if (TryAdaptInner(parser, output, target, context, 0, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Could not adapt to target type. (Output = {output.SimpleName()}, Target = {target.SimpleName()})");
    }

    /// <summary>
    /// Adapt from one type to another.
    /// </summary>
    /// <param name="from">Input type.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <returns>Type adapter, or null if no conversion is needed between types.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static IParser? Adapt(Type from, Type target, IParseContext context)
    {
        Parse.Verbose($"Adapting {from} to {target}");
        if (TryAdaptInner(null, from, target, context, 0, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Could not adapt to target type. (Type = {from.SimpleName()}, Target = {target.SimpleName()})");
    }

    /// <summary>
    /// Try to adapt a parser to a specific output type.
    /// </summary>
    /// <param name="parser">Current parser.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="convert">Adapter.</param>
    /// <returns>True if the parser was adapted, false otherwise.</returns>
    public static bool TryAdapt(IParser parser, Type target, IParseContext context, out IParser convert)
    {
        var outputType = ParseUtil.GetParserTypesOf(parser).OutputType;
        Parse.Verbose($"Try adapt parser {parser.GetType()} ({outputType}) to {target}");
        return TryAdaptInner(parser, outputType, target, context, 0, out convert);
    }

    /// <summary>
    /// Get a parser that converts from one type to another type.
    ///
    /// If the resulting parser is null, then no conversion is needed to adapt.
    /// </summary>
    /// <param name="from">Input type.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="convert">Adapter.</param>
    /// <returns>True if the type was adapted, false otherwise.</returns>
    public static bool TryAdapt(Type from, Type target, IParseContext context, out IParser? convert)
    {
        Parse.Verbose($"Try adapt {from} to {target}");
        return TryAdaptInner(null, from, target, context, 0, out convert);
    }
    
    /// <summary>
    /// Make sure a type constructor is ready to accept a tuple as an argument.
    ///
    /// If the parser does not accept a tuple, it is adapted to accept a 1-tuple.
    /// </summary>
    /// <param name="constructor"></param>
    /// <returns></returns>
    public static IParser ProcessConstructor(IParser constructor)
    {
        var input = ParseUtil.GetParserTypesOf(constructor).InputType;
        Parse.VerboseIf(!input.IsTupleType(), $"Wrapping input type ({input}) to accept 1-tuple.");
        return input.IsTupleType() ? constructor : ParseJoin.Create(TupleAdapter.UnwrapSingle(input), constructor);
    }

    /// <summary>
    /// Join two possibly-null parsers.
    /// If the first parser is null, then the second parser is returned.
    /// If the second parser is null, then the first parser is returned.
    /// </summary>
    /// <param name="first">First parser, possibly null.</param>
    /// <param name="second">Second parser.</param>
    /// <returns>Joined parser.</returns>
    [return: NotNullIfNotNull(nameof(first))]
    [return: NotNullIfNotNull(nameof(second))]
    public static IParser? MaybeJoin(IParser? first, IParser? second)
    {
        if (first is null) return second;
        return second is null ? first : ParseJoin.Create(first, second);
    }
    
    /// <summary>
    /// Inner-join two possibly-null parsers.
    /// If the first parser is null, then the second parser is returned at the given
    /// enumerable level.
    /// If the second parser is null, then the first parser is returned.
    /// </summary>
    /// <param name="first">First parser, possibly null.</param>
    /// <param name="second">Second parser.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="level">Inner join level.</param>
    /// <returns>Joined parser.</returns>
    [return: NotNullIfNotNull(nameof(first))]
    [return: NotNullIfNotNull(nameof(second))]
    public static IParser? MaybeInnerJoin(IParser? first, IParser? second, IParseContext context, int level)
    {
        if (first is null) return second?.AddLevels(level);
        return second is null ? first : ParseJoin.InnerJoin(first, second, level, context);
    }

    /// <summary>
    /// Recursively adapt a parser to a specific output type.
    /// 
    /// If the output can be directly assigned to the target type, the original parser
    /// is returned.
    /// 
    /// If there is an adapter from the output type to the target type, then
    /// the adapter is joined to the parser.
    /// </summary>
    /// <param name="parser">Current parser.</param>
    /// <param name="output">Current output type.</param>
    /// <param name="target">Current target type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="level">Nest level.</param>
    /// <param name="result">Adapted parser.</param>
    /// <returns>Parser adapted to the target type.</returns>
    /// <exception cref="ArgumentException">If no way could be detected to adapt the parser
    /// to the target type.</exception>
    public static bool TryAdaptInner(IParser? parser, Type output, Type target, IParseContext context, int level, [NotNullIfNotNull(nameof(parser))] out IParser? result)
    {
        // Check if directly assignable
        if (output.IsAssignableTo(target))
        {
            Parse.Verbose($"{output} is directly assignable to {target}");
            result = parser;
            return true;
        }

        // Try using an adapter.
        if (context.TryLookupAdapter(output, target, out var convert))
        {
            Parse.Verbose($"Adapted {output} to {target} using {convert.GetType()}");
            result = MaybeInnerJoin(parser, convert, context, level);
            return true;
        }

        // Try tuple conversions
        if (TryAdaptTuple(output, target, context, out var tupleAdapt))
        {
            result = MaybeInnerJoin(parser, tupleAdapt, context, level);
            return true;
        }
        
        // Try enumerable conversions
        return TryAdaptInnerEnumerable(parser, output, target, context, level, out result);
    }

    /// <summary>
    /// Try to perform tuple conversions on the types.
    ///
    /// If the input and target are tuples, then try to use construction if going
    /// from larger to smaller, otherwise try adapting by cutting.
    ///
    /// If going from tuple to a type, try construction.
    ///
    /// If going from a type to a tuple, try unpacking.
    /// </summary>
    /// <param name="from">Input type.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="tupleAdapt">Adapted tuple parser.</param>
    /// <returns>True if a tuple conversion was performed, false otherwise.</returns>
    private static bool TryAdaptTuple(Type from, Type target, IParseContext context, out IParser tupleAdapt)
    {
        var fromTuple = from.TryGetTupleTypes(out var fromTypes);
        var toTuple = target.TryGetTupleTypes(out var toTypes);

        if (!fromTuple && !toTuple)
        {
            tupleAdapt = default!;
            return false;
        }
        
        // Check for tuple to tuple conversion
        if (fromTuple && toTuple)
        {
            // Try to go from larger to smaller tuple using construction
            if (fromTypes.Length > toTypes.Length && TryAdaptTupleConstruct(from, fromTypes, toTypes, context, out tupleAdapt))
            {
                Parse.Verbose($"Adapted {from} to {target} by compressing -> {tupleAdapt.GetType()}");
                return true;
            }

            // Try dropping extra elements and adapting each type
            if (TryAdaptTupleCut(from, fromTypes, target, context, out var tupleCutAdapt) && tupleCutAdapt is not null)
            {
                Parse.Verbose($"Adapted {from} to {target} by cut -> {tupleCutAdapt.GetType()}");
                tupleAdapt = tupleCutAdapt;
                return true;
            }
        }

        // Try to convert from tuple to target using construction
        if (fromTuple
            && context.TryLookupType(target, out var targetDescriptor)
            && targetDescriptor.TryConstruct(target, context, fromTypes, out tupleAdapt))
        {
            Parse.Verbose($"Converting {from} to {target} via construction: {tupleAdapt.GetType()}");
            // Slice the input if the constructor took less elements
            var input = ParseUtil.GetParserTypesOf(tupleAdapt).InputType;
            if (input.IsTupleType())
            {
                var size = input.GetTupleSize();
                if (size < fromTypes.Length)
                {
                    Parse.Verbose($"Slicing {from} to size {size}");
                    tupleAdapt = ParseJoin.Create(TupleAdapter.Slice(from, 0, size), tupleAdapt);
                }
            }
            else
            {
                Parse.Verbose($"by taking first element of {from}");
                // If the constructor is not a tuple, then take the first element from the tuple.
                // This special handling is used instead of ProcessConstructor
                var first = TupleAdapter.First(from);
                tupleAdapt = ParseJoin.Create(first, tupleAdapt);
            }
            
            return true;
        }
        
        // Try to unwrap a 1-tuple
        if (fromTuple
            && fromTypes.Length == 1
            && TryAdaptInner(null, fromTypes[0], target, context, 0, out var conversion))
        {
            Parse.Verbose($"Unwrapping 1-tuple {from} -> {conversion?.GetType()}");
            tupleAdapt = MaybeJoin(TupleAdapter.UnwrapSingle(fromTypes[0]), conversion);
            return true;
        }
        
        // Try to convert tuple to object by taking first item
        if (fromTuple
            && !toTuple
            && TryAdapt(fromTypes[0], target, context, out var firstAdapt))
        {
            Parse.Verbose($"Taking first item of {from} -> {firstAdapt?.GetType()}");
            tupleAdapt = MaybeJoin(TupleAdapter.First(from), firstAdapt);
            return true;
        }
        
        // Try to convert from type to tuple using unpacking
        if (toTuple
            && context.TryLookupType(from, out var fromDescriptor)
            && fromDescriptor.TryUnpack(from, context, toTypes.Length, out tupleAdapt))
        {
            Parse.Verbose($"Unpacking {from} -> {tupleAdapt.GetType()}");
            return true;
        }

        tupleAdapt = default!;
        return false;
    }

    /// <summary>
    /// Try adapting a larger tuple to a smaller tuple using construction on the
    /// target elements.
    /// </summary>
    /// <param name="from">Input tuple.</param>
    /// <param name="fromTypes">Input tuple types.</param>
    /// <param name="toTypes">Target tuple types.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="result">Tuple construction adapter.</param>
    /// <returns>True if the target tuple can be adapted by construction, false otherwise.</returns>
    private static bool TryAdaptTupleConstruct(Type from, Type[] fromTypes, Type[] toTypes, IParseContext context, out IParser result)
    {
        var chunks = new TupleChunkParse[toTypes.Length];
        ReadOnlySpan<Type> current = fromTypes;
        
        Parse.Verbose($"Try adapt tuple {from} to types {Types.CreateTupleType(toTypes)}");

        var offset = 0;
        for (var i = 0; i < toTypes.Length; i++)
        {
            var type = toTypes[i];
            if (!context.TryLookupType(type, out var targetInner))
            {
                Parse.Verbose($"Failed at element {i}: No type descriptor");
                result = default!;
                return false;
            }
            if (!targetInner.TryConstruct(type, context, current, out var constructor))
            {
                Parse.Verbose($"Failed at element {i}: {type} could not be constructed");
                result = default!;
                return false;
            }
            
            constructor = ProcessConstructor(constructor);

            // Cut and adapt to constructor tuple
            var input = ParseUtil.GetParserTypesOf(constructor).InputType;
            if (!TryAdaptTupleCut(from, current, input, context, out var constructorAdapter))
            {
                result = default!;
                return false;
            }

            Parse.Verbose($"Adapted element {i} -> {constructorAdapter?.GetType()}");
            context.ApplyDisambiguation(null);
            var inputTypes = input.GetTupleTypes();
            chunks[i] = new TupleChunkParse(offset, inputTypes, MaybeJoin(constructorAdapter, constructor));

            offset += inputTypes.Length;
            current = current[inputTypes.Length..];
        }

        result = TupleAdapter.Compress(from, chunks);
        return true;
    }

    /// <summary>
    /// Try adapting from one tuple type to another by dropping excess elements
    /// and adapting the remaining elements.
    /// </summary>
    /// <param name="from">Input tuple.</param>
    /// <param name="fromTypesFull">Input tuple types.</param>
    /// <param name="target">Target tuple.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="result">Tuple cut adapter, or null if the source and target is the same type.</param>
    /// <returns>True if there is a cutting conversion from the input to the output, false otherwise.</returns>
    private static bool TryAdaptTupleCut(Type from, ReadOnlySpan<Type> fromTypesFull, Type target, IParseContext context, out IParser? result)
    {
        var targetTypes = target.GetTupleTypes();

        // Cannot cut from smaller to larger
        if (fromTypesFull.Length < targetTypes.Length)
        {
            result = default;
            return false;
        }
        
        var fromTypes = fromTypesFull[..targetTypes.Length];
        var fromTuple = Types.CreateTupleType(fromTypes);
        
        Parse.Verbose($"Try adapt tuple {from} to tuple {target}");
    
        if (fromTuple == target)
        {
            // Slice if larger
            if (fromTypesFull.Length > targetTypes.Length)
            {
                Parse.Verbose($"Direct slice tuple {from} to {fromTuple}");
                result = TupleAdapter.Slice(from, 0, targetTypes.Length);
                return true;
            }
            
            // No conversion needed
            result = default;
            return true;
        }
    
        // Adapt each element
        var parsers = new IParser[targetTypes.Length];
        for (var i = 0; i < parsers.Length; i++)
        {
            if (!TryAdaptInner(null, fromTypes[i], targetTypes[i], context, 0, out var elementParser))
            {
                Parse.Verbose($"Failed at element {i}: Could not adapt {fromTypes[i]} to {targetTypes[i]}");
                result = default;
                return false;
            }

            Parse.Verbose($"Adapted element {i} -> {elementParser?.GetType()}");
            context.ApplyDisambiguation(null);
            parsers[i] = elementParser ?? IdentityAdapter.Create(targetTypes[i]);
        }

        result = TupleAdapter.Create(fromTypes, targetTypes, parsers);
        if (fromTypesFull.Length > targetTypes.Length)
        {
            Parse.Verbose($"Slicing tuple {from} to {fromTuple}");
            result = ParseJoin.Create(TupleAdapter.Slice(from, 0, targetTypes.Length), result);
        }
        return true;
    }

    /// <summary>
    /// Check for enumerable conversions when adapting.
    ///
    /// This adapts in cases where the output type is enumerable and the target type
    /// is either <see cref="IEnumerable{T}"/> or a collectable type.
    /// </summary>
    /// <param name="parser">Current parser.</param>
    /// <param name="output">Current output type.</param>
    /// <param name="target">Current target type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="level">Nest level.</param>
    /// <param name="result">Adapted parser.</param>
    /// <returns></returns>
    private static bool TryAdaptInnerEnumerable(IParser? parser, Type output, Type target, IParseContext context, int level, [NotNullIfNotNull(nameof(parser))] out IParser? result)
    {
        // Check if the output is enumerable
        if (!ParseUtil.TryGetInnerType(output, context, out var outputInner, out var selector))
        {
            result = default;
            return false;
        }
        
        Parse.Verbose($"Try using {output} as IEnumerable<{outputInner}>");

        // If the target is the same as the element type, just call .First()
        if (outputInner.IsAssignableTo(target))
        {
            Parse.Verbose($"Adapting enumerable {parser?.GetType()} by calling .First() -> {selector?.GetType()}");
            result = MaybeInnerJoin(parser, MaybeJoin(selector, EnumerableAdapter.First(outputInner)), context, level);
            return true;
        }
        
        // This is a special case of the next check.
        // If the target is IEnumerable<>, then no collector is needed.
        // This avoids the need to insert an "identity" parser.
        if (target.Generic() == typeof(IEnumerable<>))
        {
            result = MaybeInnerJoin(parser, selector, context, level);

            var enumerableInner = target.GetSingleTypeArgument();
            if (!TryAdapt(outputInner, enumerableInner, context, out var innerAdapt))
            {
                Parse.Verbose($"Failed to use {output} as IEnumerable<{outputInner}>: Could not adapt {outputInner} to {enumerableInner}");
                return false;
            }
            
            Parse.Verbose($"Adapting enumerable {parser?.GetType()} to IEnumerable<{target}> -> {innerAdapt?.GetType()}");
            result = MaybeInnerJoin(result, innerAdapt, context, level + 1);
            return true;
        }

        var targetDescriptor = context.TryLookupType(target, out var descriptor);

        // If the target is collectable then try to adapt the inner type and collect it.
        if (targetDescriptor
            && descriptor.TryCollectSelf(target, context, out var targetInner, out var constructor))
        {
            var disambiguationAvailable = context.ApplyDisambiguation(typeof(Collect<>));
            var preferConstruct = disambiguationAvailable && context.ApplyDisambiguation(typeof(Construct));
            Debug.Assert(!disambiguationAvailable || preferConstruct);
            Parse.VerboseIf(preferConstruct, "Using disambiguation to prefer construction.");
            
            var joined = MaybeInnerJoin(parser, selector, context, level);
            
            // Try to adapt output inner type to target inner type
            if (!preferConstruct && TryAdaptInner(joined, outputInner, targetInner, context, level + 1, out var enumerableAdapt))
            {
                Parse.Verbose($"Adapted enumerable {parser?.GetType()} to {target} -> {enumerableAdapt?.GetType()}");
                result = MaybeInnerJoin(enumerableAdapt, constructor, context, level);
                return true;
            }
            
            // Try adapt enumerable to container of tuple
            if (TryAdaptEnumerableTuple(targetInner, outputInner, context, out var innerConstructor))
            {
                Parse.Verbose($"Adapted enumerable {parser?.GetType()} to {target} of tuple {targetInner} -> {innerConstructor.GetType()}");
                result = MaybeInnerJoin(joined, EnumerableAdapter.ConstructInnerTuple(outputInner, constructor, innerConstructor), context, level);
                return true;
            }
            
            // Try to construct the inner type
            if (context.TryLookupType(targetInner, out var innerDescriptor)
                && innerDescriptor.TryConstruct(targetInner, context, new TypeSpan(in outputInner), out innerConstructor))
            {
                Parse.Verbose($"Adapted enumerable {parser?.GetType()} to {target} by constructing {targetInner} -> {innerConstructor.GetType()}");
                result = MaybeInnerJoin(joined, EnumerableAdapter.ConstructInner(outputInner, constructor, innerConstructor), context, level);
                return true;
            }
        }

        // Enumerable to single item
        if (targetDescriptor
            && descriptor.TryConstruct(target, context, new TypeSpan(in outputInner), out var constructAdapt))
        {
            Parse.Verbose($"Adapted enumerable {parser?.GetType()} to {target} via construction -> {constructAdapt.GetType()}");
            var toSingle = MaybeJoin(selector, EnumerableAdapter.ConstructSingle(outputInner, ProcessConstructor(constructAdapt)));
            result = MaybeInnerJoin(parser, toSingle, context, level);
            return true;
        }
        
        // Enumerable to tuple
        if (TryAdaptEnumerableTuple(target, outputInner, context, out var enumerableToTuple))
        {
            Parse.Verbose($"Adapted enumerable {parser?.GetType()} to tuple {target} -> {enumerableToTuple.GetType()}");
            result = MaybeInnerJoin(parser, MaybeJoin(selector, enumerableToTuple), context, level);
            return true;
        }

        Parse.Verbose($"Failed to use {output} as IEnumerable<{outputInner}>");
        result = default;
        return false;
    }

    /// <summary>
    /// Check if an enumerable can be converted to a tuple.
    /// </summary>
    /// <param name="target">Target type.</param>
    /// <param name="outputInner">Output inner type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="result">Adapted parser.</param>
    /// <returns></returns>
    private static bool TryAdaptEnumerableTuple(Type target, Type outputInner, IParseContext context, out IParser result)
    {
        if (!target.TryGetTupleTypes(out var tupleTypes))
        {
            result = default!;
            return false;
        }
        
        Parse.Verbose($"Try adapt IEnumerable<{outputInner}> to {target}");

        var sections = new IParser[tupleTypes.Length];

        for (var i = 0; i < tupleTypes.Length; i++)
        {
            var elementType = tupleTypes[i];

            // Try to parse a nested tuple
            if (TryAdaptEnumerableTuple(elementType, outputInner, context, out var innerTuple))
            {
                Parse.Verbose($"Adapted nested tuple at index {i} -> {innerTuple.GetType()}");
                sections[i] = innerTuple;
                continue;
            }

            // Try to adapt a single value to the element type.
            if (TryAdapt(outputInner, elementType, context, out var elementAdapt))
            {
                Parse.Verbose($"Adapted element {i} -> {elementAdapt?.GetType()}");
                sections[i] = EnumerableAdapter.PartialSingle(outputInner, elementAdapt);
                continue;
            }
                
            // Take many values and try using construction.
            if (context.TryLookupType(elementType, out var elementDescriptor)
                && elementDescriptor.TryConstruct(elementType, context, new TypeSpan(in outputInner), out var elementConstructor))
            {
                Parse.Verbose($"Adapted element {i} via construction -> {elementConstructor.GetType()}");
                sections[i] = EnumerableAdapter.PartialTake(outputInner, ProcessConstructor(elementConstructor));
                continue;
            }

            Parse.Verbose($"Failed at element {i}: Could not create {elementType}");
            result = default!;
            return false;
        }

        result = EnumerableAdapter.ToTuple(outputInner, sections);
        return true;
    }
}