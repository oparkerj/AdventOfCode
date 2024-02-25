using System.Diagnostics.CodeAnalysis;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// This class implements the algorithm used to adapt one type to another
/// in the parsing library.
///
/// The algorithm will try to convert one type to another in the following order:
///
/// - Direct assignment (no conversion)
/// - Using adapter from context
/// ~ Tuple to tuple conversion:
///   - If going larger to smaller, try constructing each target element
///   - Trim input to same size as output
///   - Trim input and adapt element-wise
/// - Adapt tuple to object via construction
/// - Unwrap 1-tuple and adapt to target
/// - Adapt object to tuple via unpacking
/// ~ Enumerable conversion:
///   - If target is same as element type, just call .First()
///   - If target is IEnumerable, adapt element type
///   - Try to collect the target type
///   - Create target via construction
///   ~ Adapt enumerable to tuple
///     - Adapt element to tuple component
///     - Construct tuple component
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
    public static IParser Adapt(IParser parser, Type target, IReadOnlyParseContext context)
    {
        var output = ParseUtil.GetParserTypesOf(parser).OutputType;
        if (TryAdaptInner(parser, output, target, context, 0, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Could not adapt to target type. (Output = {output.SimpleName()}, Target = {target.SimpleName()})");
    }

    public static bool TryAdapt(IParser parser, Type target, IReadOnlyParseContext context, out IParser convert)
    {
        var outputType = ParseUtil.GetParserTypesOf(parser).OutputType;
        return TryAdaptInner(parser, outputType, target, context, 0, out convert);
    }

    public static bool TryAdapt(Type from, Type target, IReadOnlyParseContext context, out IParser? convert)
    {
        return TryAdaptInner(null, from, target, context, 0, out convert);
    }
    
    public static IParser ProcessConstructor(IParser constructor)
    {
        var input = ParseUtil.GetParserTypesOf(constructor).InputType;
        return input.IsTupleType() ? constructor : ParseJoin.Create(TupleAdapter.UnwrapSingle(input), constructor);
    }

    /// <summary>
    /// Inner-join a possibly-null parser to a second parser.
    /// If the first parser is null, then the second parser is returned at the given
    /// enumerable level.
    /// </summary>
    /// <param name="first">First parser, possibly null.</param>
    /// <param name="second">Second parser.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="level">Inner join level.</param>
    /// <returns>Joined parser.</returns>
    public static IParser MaybeJoin(IParser? first, IParser second, IReadOnlyParseContext context, int level = 0)
    {
        return first is null ? second.AddLevels(level) : ParseJoin.InnerJoin(first, second, level, context);
    }

    /// <summary>
    /// Recursively adapt a parser to a specific output type.
    /// 
    /// If the output can be directly assigned to the target type, the original parser
    /// is returned.
    /// 
    /// If there is an adapter from the output type to the target type, then
    /// the adapter is joined to the parser.
    ///
    /// TODO Description
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
    public static bool TryAdaptInner(IParser? parser, Type output, Type target, IReadOnlyParseContext context, int level, [NotNullIfNotNull(nameof(parser))] out IParser? result)
    {
        // Check if directly assignable
        if (output.IsAssignableTo(target))
        {
            result = parser;
            return true;
        }

        // Try using an adapter.
        if (context.TryLookupAdapter(output, target, out var convert))
        {
            result = MaybeJoin(parser, convert, context, level);
            return true;
        }

        // Try tuple conversions
        if (TryAdaptTuple(output, target, context, out var tupleAdapt))
        {
            result = MaybeJoin(parser, tupleAdapt, context, level);
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
    private static bool TryAdaptTuple(Type from, Type target, IReadOnlyParseContext context, out IParser tupleAdapt)
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
                return true;
            }

            // Try dropping extra elements and adapting each type
            if (TryAdaptTupleCut(from, fromTypes, target, context, out var tupleCutAdapt) && tupleCutAdapt is not null)
            {
                tupleAdapt = tupleCutAdapt;
                return true;
            }
        }

        // Try to convert from tuple to target using construction
        if (fromTuple
            && context.TryLookupType(target, out var targetDescriptor)
            && targetDescriptor.TryConstruct(target, context, fromTypes, out tupleAdapt))
        {
            // Slice the input if the constructor took less elements
            var input = ParseUtil.GetParserTypesOf(tupleAdapt).InputType;
            if (input.IsTupleType())
            {
                var size = input.GetTupleSize();
                if (size < fromTypes.Length)
                {
                    tupleAdapt = ParseJoin.Create(TupleAdapter.Slice(from, 0, size), tupleAdapt);
                }
            }
            else
            {
                // If the constructor is not a tuple, then take the first element from the tuple.
                // This special handling is used instead of ProcessConstructor
                var unwrap = TupleAdapter.First(from);
                tupleAdapt = ParseJoin.Create(unwrap, tupleAdapt);
            }
            
            return true;
        }
        
        // Try to unwrap a 1-tuple
        if (fromTuple
            && fromTypes.Length == 1
            && TryAdaptInner(null, fromTypes[0], target, context, 0, out var conversion))
        {
            tupleAdapt = TupleAdapter.UnwrapSingle(fromTypes[0]);
            if (conversion is not null)
            {
                tupleAdapt = ParseJoin.Create(tupleAdapt, conversion);
            }
            return true;
        }
        
        // Try to convert from type to tuple using unpacking
        if (toTuple
            && context.TryLookupType(from, out var fromDescriptor)
            && fromDescriptor.TryUnpack(from, context, toTypes.Length, out tupleAdapt))
        {
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
    private static bool TryAdaptTupleConstruct(Type from, Type[] fromTypes, Type[] toTypes, IReadOnlyParseContext context, out IParser result)
    {
        var chunks = new TupleChunkParse[toTypes.Length];
        ReadOnlySpan<Type> current = fromTypes;

        var offset = 0;
        for (var i = 0; i < toTypes.Length; i++)
        {
            var type = toTypes[i];
            if (!context.TryLookupType(type, out var targetInner))
            {
                result = default!;
                return false;
            }
            if (!targetInner.TryConstruct(type, context, current, out var constructor))
            {
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

            var inputTypes = input.GetTupleTypes();
            chunks[i] = new TupleChunkParse(offset, inputTypes, MaybeJoin(constructorAdapter, constructor, context));

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
    private static bool TryAdaptTupleCut(Type from, ReadOnlySpan<Type> fromTypesFull, Type target, IReadOnlyParseContext context, out IParser? result)
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
    
        if (fromTuple == target)
        {
            // Slice if larger
            if (fromTypesFull.Length > targetTypes.Length)
            {
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
                result = default;
                return false;
            }

            parsers[i] = elementParser ?? IdentityAdapter.Create(targetTypes[i]);
        }

        result = TupleAdapter.Create(fromTypes, targetTypes, parsers);
        if (fromTypesFull.Length > targetTypes.Length)
        {
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
    private static bool TryAdaptInnerEnumerable(IParser? parser, Type output, Type target, IReadOnlyParseContext context, int level, [NotNullIfNotNull(nameof(parser))] out IParser? result)
    {
        // Check if the output is enumerable
        if (!ParseUtil.TryGetInnerType(output, context, out var outputInner, out var selector))
        {
            result = default;
            return false;
        }

        // If the target is the same as the element type, just call .First()
        if (outputInner.IsAssignableTo(target))
        {
            result = parser;
            if (selector is not null)
            {
                result = MaybeJoin(result, selector, context, level);
            }
            result = MaybeJoin(result, EnumerableAdapter.First(outputInner), context, level);
            return true;
        }
        
        // This is a special case of the next check.
        // If the target is IEnumerable<>, then no collector is needed.
        // This avoids the need to insert an "identity" parser.
        if (target.Generic() == typeof(IEnumerable<>))
        {
            result = parser;
            if (selector is not null)
            {
                result = MaybeJoin(result, selector, context, level);
            }

            var enumerableInner = target.GetSingleTypeArgument();
            if (!TryAdapt(outputInner, enumerableInner, context, out var innerAdapt)) return false;
            if (innerAdapt is not null)
            {
                result = MaybeJoin(result, innerAdapt.AddLevels(1), context, level);
            }
            return true;
        }

        var targetDescriptor = context.TryLookupType(target, out var descriptor);

        // If the target is collectable then try to adapt the inner type and collect it.
        if (targetDescriptor
            && descriptor.TryCollectSelf(target, context, out var targetInner, out var constructor))
        {
            var joined = parser;
            if (selector is not null)
            {
                joined = MaybeJoin(joined, selector, context, level);
            }
            if (TryAdaptInner(joined, outputInner, targetInner, context, level + 1, out var enumerableAdapt))
            {
                result = MaybeJoin(enumerableAdapt, constructor, context, level);
                return true;
            }
        }

        // Enumerable to single item
        if (targetDescriptor
            && descriptor.TryConstruct(target, context, new TypeSpan(in outputInner), out var constructAdapt))
        {
            result = parser;
            if (selector is not null)
            {
                result = MaybeJoin(result, selector, context, level);
            }
            constructAdapt = ProcessConstructor(constructAdapt);
            result = MaybeJoin(result, EnumerableAdapter.Single(outputInner, constructAdapt), context, level);
            return true;
        }
        
        // Enumerable to tuple
        if (TryAdaptEnumerableTuple(target, outputInner, context, out var enumerableToTuple))
        {
            result = parser;
            if (selector is not null)
            {
                result = MaybeJoin(result, selector, context, level);
            }
            result = MaybeJoin(result, enumerableToTuple, context, level);
            return true;
        }

        result = default;
        return false;
    }

    private static bool TryAdaptEnumerableTuple(Type target, Type outputInner, IReadOnlyParseContext context, out IParser result)
    {
        if (!target.TryGetTupleTypes(out var tupleTypes))
        {
            result = default!;
            return false;
        }

        var sections = new IParser[tupleTypes.Length];

        for (var i = 0; i < tupleTypes.Length; i++)
        {
            var elementType = tupleTypes[i];

            if (TryAdapt(outputInner, elementType, context, out var elementAdapt))
            {
                var adapt = TupleAdapter.UnwrapSingle(outputInner);
                if (elementAdapt is not null)
                {
                    adapt = ParseJoin.Create(adapt, elementAdapt);
                }
                sections[i] = adapt;
                continue;
            }
                
            if (context.TryLookupType(elementType, out var elementDescriptor)
                && elementDescriptor.TryConstruct(elementType, context, new TypeSpan(in outputInner), out var elementConstructor))
            {
                sections[i] = ProcessConstructor(elementConstructor);
                continue;
            }

            result = default!;
            return false;
        }

        result = EnumerableAdapter.Create(outputInner, sections);
        return true;
    }
}