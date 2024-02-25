using System.Diagnostics;
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
}