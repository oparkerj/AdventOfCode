using System.Runtime.CompilerServices;
using AdventToolkit2.Collections;
using AdventToolkit2.Parsing.Interface;
using AdventToolkit2.Reflect;

namespace AdventToolkit2.Parsing.Builtin;

/// <summary>
/// Methods to create a split parse.
/// </summary>
public static class SplitParse
{
    /// <summary>
    /// Create a parser that splits a type into a tuple.
    /// </summary>
    /// <param name="type">Input type.</param>
    /// <param name="size">Tuple size.</param>
    /// <returns></returns>
    public static IParser Create(Type type, int size)
    {
        var output = Types.CreateTupleType(type, size);
        return typeof(SplitParse<,>).NewParserGeneric([type, output], size);
    }
}

/// <summary>
/// Parser that takes an input and creates a tuple where all elements
/// have the same value.
/// </summary>
/// <param name="size">Tuple size</param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class SplitParse<TIn, TOut>(int size) : IParser<TIn, TOut>
    where TOut : ITuple
{
    public TOut Parse(TIn input)
    {
        using var buffer = Arr<object?>.Of(input, size);
        return (TOut) Types.CreateTupleFrom(typeof(TOut), buffer);
    }
}