using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// Helper functions for parsing.
/// </summary>
public static class ParseUtil
{
    /// <summary>
    /// Create an instance of a parser.
    /// </summary>
    /// <param name="type">Parser type.</param>
    /// <param name="args">Constructor arguments.</param>
    /// <returns>Parser instance.</returns>
    public static IParser NewParser(this Type type, params object?[] args)
    {
        return type.New<IParser>(args);
    }
    
    /// <summary>
    /// Create an instance of a parser. 
    /// </summary>
    /// <param name="type">Parser type.</param>
    /// <param name="generic">Generic arguments.</param>
    /// <param name="args">Constructor arguments.</param>
    /// <returns>Parser instance.</returns>
    public static IParser NewParserGeneric(this Type type, Type[] generic, params object?[] args)
    {
        return type.NewGeneric<IParser>(generic, args);
    }
    
    /// <summary>
    /// Try to get the input and output types from a parser type.
    /// </summary>
    /// <param name="type">Parser type.</param>
    /// <param name="input">Input type.</param>
    /// <param name="output">Output type</param>
    /// <returns>True if the type is a parser type, false otherwise.</returns>
    public static bool TryGetParserTypes(Type type, out Type input, out Type output)
    {
        if (type.TryGetTypeArguments(typeof(IParser<,>), out var types))
        {
            input = types[0];
            output = types[1];
            return true;
        }

        input = default!;
        output = default!;
        return false;
    }

    /// <summary>
    /// Try to get the input and output types from a parser object.
    /// </summary>
    /// <param name="o">Parser.</param>
    /// <param name="input">Input type.</param>
    /// <param name="output">Output type.</param>
    /// <returns>True if the object is a parser type, false otherwise.</returns>
    public static bool TryGetParserTypesOf(object o, out Type input, out Type output)
    {
        return TryGetParserTypes(o.GetType(), out input, out output);
    }
    
    /// <summary>
    /// Get the input and output types from a parser type.
    /// </summary>
    /// <param name="type">Parser type.</param>
    /// <returns>Input and output types.</returns>
    public static (Type InputType, Type OutputType) GetParserTypes(Type type)
    {
        var types = type.GetTypeArguments(typeof(IParser<,>));
        return (types[0], types[1]);
    }
    
    /// <summary>
    /// Get the input and output types from a parser object.
    /// </summary>
    /// <param name="o">Parser object.</param>
    /// <returns>Input and output types.</returns>
    public static (Type InputType, Type OutputType) GetParserTypesOf(object o)
    {
        return GetParserTypes(o.GetType());
    }

    /// <summary>
    /// Wrap the parser in SelectAdapters to achieve a certain enumerable level.
    /// </summary>
    /// <param name="start">Current parser.</param>
    /// <param name="level">Target enumerable level.</param>
    /// <returns>Wrapped parser.</returns>
    public static IParser AddLevels(this IParser start, int level)
    {
        var enumerable = typeof(IEnumerable<>);
        var (input, output) = GetParserTypesOf(start);

        while (level-- > 0)
        {
            start = typeof(SelectAdapter<,>).NewParserGeneric([input, output], start);
            input = enumerable.MakeGenericType(input);
            output = enumerable.MakeGenericType(output);
        }

        return start;
    }

    /// <summary>
    /// Try to get a collector to construct the type, if it is collectable.
    /// </summary>
    /// <param name="descriptor">Current type descriptor.</param>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="inner">Element type.</param>
    /// <param name="constructor">Parser that converts a sequence of the element type
    /// to an instance of the type.</param>
    /// <returns>True if the type can be collected, false otherwise.</returns>
    public static bool TryCollectSelf(this ITypeDescriptor descriptor, Type type, IReadOnlyParseContext context, out Type inner, out IParser constructor)
    {
        Debug.Assert(descriptor.Match(type));
        
        constructor = default!;
        return descriptor.TryGetCollectType(type, context, out inner)
               && descriptor.TryCollect(type, inner, context, out constructor);
    }
    
    /// <summary>
    /// Get the element type from a type descriptor.
    /// If the type is enumerable or the type descriptor defines a way to enumerate
    /// the type, this will return the element type and a parser which can obtain an enumerable
    /// from the input type.
    /// </summary>
    /// <param name="descriptor">Type descriptor.</param>
    /// <param name="type">Current type. Must be valid for the descriptor.</param>
    /// <param name="inner">Element type.</param>
    /// <param name="selector">Parser that converts from the input type to an enumerable.</param>
    /// <returns>True if the type can be enumerated, false otherwise.</returns>
    public static bool TryGetInnerType(this ITypeDescriptor descriptor, Type type, out Type inner, out IParser? selector)
    {
        Debug.Assert(descriptor.Match(type));
        
        if (type.TryGetTypeArguments(typeof(IEnumerable<>), out var enumerableTypes))
        {
            inner = enumerableTypes[0];
            selector = default;
            return true;
        }

        return descriptor.TrySelect(type, out inner, out selector);
    }
    
    /// <summary>
    /// Try to enumerate the given type.
    /// </summary>
    /// <param name="type">Current type.</param>
    /// <param name="context">Parse context.</param>
    /// <param name="inner">Element type.</param>
    /// <param name="selector">Parser that converts from the input type to an enumerable.</param>
    /// <returns>True if the type can be enumerated, false otherwise.</returns>
    public static bool TryGetInnerType(Type type, IReadOnlyParseContext context, out Type inner, out IParser? selector)
    {
        if (type.TryGetTypeArguments(typeof(IEnumerable<>), out var enumerableTypes))
        {
            inner = enumerableTypes[0];
            selector = default;
            return true;
        }

        if (context.TryLookupType(type, out var descriptor))
        {
            return descriptor.TrySelect(type, out inner, out selector);
        }

        inner = default!;
        selector = default;
        return false;
    }

    /// <summary>
    /// Adapt a parser to a specific output type.
    /// </summary>
    /// <param name="parser">Current parser.</param>
    /// <param name="target">Target type.</param>
    /// <param name="context">Parse context.</param>
    /// <returns></returns>
    public static IParser Adapt(IParser parser, Type target, IReadOnlyParseContext context)
    {
        var output = GetParserTypesOf(parser).OutputType;
        if (TryAdaptInner(parser, output, target, context, 0, out var result))
        {
            return result;
        }
        throw new ArgumentException($"Could not adapt to target type. (Output = {output.SimpleName()}, Target = {target.SimpleName()})");
    }

    /// <summary>
    /// Inner-join a possibly-null parser to a second parser.
    /// </summary>
    /// <param name="first">First parser, possibly null.</param>
    /// <param name="second">Second parser.</param>
    /// <param name="level">Inner join level.</param>
    /// <param name="context">Parse context.</param>
    /// <returns>Joined parser.</returns>
    private static IParser AdaptJoin(IParser? first, IParser second, int level, IReadOnlyParseContext context)
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
    /// If the output and target type are enumerable and the target type can
    /// be constructed from a sequence of the output elements
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
            result = AdaptJoin(parser, convert, level, context);
            return true;
        }

        // Try tuple conversions
        if (TryAdaptTuple(output, target, context, out var tupleAdapt))
        {
            result = AdaptJoin(parser, tupleAdapt, level, context);
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
            var input = GetParserTypesOf(tupleAdapt).InputType;
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
                var unwrap = TupleAdapter.UnwrapSingle(TupleAdapter.Slice(from, 0, 1));
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

            // Cut and adapt to constructor tuple
            var input = GetParserTypesOf(constructor).InputType;
            if (!TryAdaptTupleCut(from, current, input, context, out var constructorAdapter))
            {
                result = default!;
                return false;
            }

            var inputTypes = input.GetTupleTypes();
            chunks[i] = new TupleChunkParse(offset, inputTypes, AdaptJoin(constructorAdapter, constructor, 0, context));

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
        if (!TryGetInnerType(output, context, out var outputInner, out var selector))
        {
            result = default;
            return false;
        }
        
        // This is a special case of the next check.
        // If the target is IEnumerable<>, then no collector is needed.
        // This avoids the need to insert an "identity" parser.
        if (target.Generic() == typeof(IEnumerable<>))
        {
            result = parser;
            if (selector is not null)
            {
                result = AdaptJoin(result, selector, level, context);
            }
            return true;
        }

        // If the target is collectable then try to adapt the inner type and collect it.
        if (context.TryLookupType(target, out var descriptor)
            && descriptor.TryCollectSelf(target, context, out var targetInner, out var constructor))
        {
            var joined = parser;
            if (selector is not null)
            {
                joined = AdaptJoin(joined, selector, level, context);
            }
            if (TryAdaptInner(joined, outputInner, targetInner, context, level + 1, out var enumerableAdapt))
            {
                result = AdaptJoin(enumerableAdapt, constructor, level, context);
                return true;
            }
            // TODO TryConstruct target inner
        }

        result = default;
        return false;
    }
}