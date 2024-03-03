using System.Diagnostics;
using AdventToolkit.New.Data;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Constructor methods for enumerable adapters.
/// </summary>
public static class EnumerableAdapter
{
    /// <summary>
    /// Take elements from en enumerator and package them into a tuple.
    /// </summary>
    /// <param name="source">Input sequence.</param>
    /// <param name="count">Item count.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Tuple of items.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static object Take<T>(IEnumerator<T> source, int count)
    {
        using var arr = Arr<object?>.Get(count);

        for (var i = 0; i < count; i++)
        {
            if (!source.MoveNext()) throw new ArgumentOutOfRangeException(nameof(source));
            arr[i] = source.Current;
        }

        return Types.CreateTuple(typeof(T), arr);
    }

    /// <summary>
    /// Get a parser that takes one element of an enumerator and passes it to the given
    /// parser. If the parser is null, the element type is directly returned.
    /// </summary>
    /// <param name="elements">Sequence type.</param>
    /// <param name="parser">Maybe-null parser.</param>
    /// <returns></returns>
    public static IParser PartialSingle(Type elements, IParser? parser)
    {
        if (parser is null) return typeof(EnumerableConstructIdentity<>).NewParserGeneric([elements]);
        var (input, output) = ParseUtil.GetParserTypesOf(parser);
        return typeof(EnumerableConstructSingle<,>).NewParserGeneric([input, output], parser);
    }

    /// <summary>
    /// Get a parser that takes a fixed number of elements from the enumerator
    /// and passes it to the constructor parser.
    /// </summary>
    /// <param name="elements">Sequence type.</param>
    /// <param name="constructor">Element constructor.</param>
    /// <returns></returns>
    public static IParser PartialTake(Type elements, IParser constructor)
    {
        var (input, output) = ParseUtil.GetParserTypesOf(constructor);
        Debug.Assert(input.IsTupleType());
        var size = input.GetTupleSize();
        return typeof(EnumerableConstructTuple<,,>).NewParserGeneric([elements, input, output], constructor, size);
    }

    /// <summary>
    /// Get a parser that returns the first element of an enumerable.
    /// </summary>
    /// <param name="type">Element type.</param>
    /// <returns></returns>
    public static IParser First(Type type)
    {
        return typeof(EnumerableFirst<>).NewParserGeneric([type]);
    }
    
    /// <summary>
    /// Get a parser that constructs an object from an enumerable.
    /// The parser input should be a tuple where all elements are the inner
    /// type of the enumerable.
    /// </summary>
    /// <param name="elements">Enumerable element type.</param>
    /// <param name="constructor">Object constructor</param>
    /// <returns></returns>
    public static IParser ConstructSingle(Type elements, IParser constructor)
    {
        var (input, output) = ParseUtil.GetParserTypesOf(constructor);
        var size = input.GetTupleSize();
        return typeof(EnumerableToValue<,,>).NewParserGeneric([elements, input, output], constructor, size);
    }

    /// <summary>
    /// Get a parser that converts an enumerable to a tuple.
    /// Elements are taken from the enumerable to construct each element
    /// of the tuple.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="parsers">Parsers which accept an <see cref="IEnumerator{T}"/> and
    /// create each element of the tuple.</param>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    public static IParser ToTuple(Type elements, ReadOnlySpan<IParser> parsers)
    {
        // Set up generic types and constructor args
        var count = Math.Min(parsers.Length, Types.PrimaryTupleSize);
        var generic = new Type[parsers.Length];
        for (var i = 0; i < count; i++)
        {
            var output = ParseUtil.GetParserTypesOf(parsers[i]).OutputType;
            generic[i] = output;
        }

        if (parsers.Length > Types.PrimaryTupleSize)
        {
            var rest = ToTuple(elements, parsers[Types.PrimaryTupleSize..]);
            var output = ParseUtil.GetParserTypesOf(rest).OutputType;
            return typeof(EnumerableToTuple<,,,,,,,,>).NewParserGeneric([elements, ..generic, output], [..parsers, rest]);
        }

        return count switch
        {
            1 => typeof(EnumerableToTuple<,>).NewParserGeneric([elements, ..generic], [..parsers]),
            2 => typeof(EnumerableToTuple<,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            3 => typeof(EnumerableToTuple<,,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            4 => typeof(EnumerableToTuple<,,,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            5 => typeof(EnumerableToTuple<,,,,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            6 => typeof(EnumerableToTuple<,,,,,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            7 => typeof(EnumerableToTuple<,,,,,,,>).NewParserGeneric([elements, ..generic], [..parsers]),
            _ => throw new UnreachableException()
        };
    }
}

/// <summary>
/// Get the first element of an enumerable.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumerableFirst<T> : IParser<IEnumerable<T>, T>
{
    public T Parse(IEnumerable<T> input) => input.First();
}

/// <summary>
/// Convert an object to a single value via construction.
/// </summary>
/// <param name="constructor"></param>
/// <param name="count"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TTuple"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class EnumerableToValue<T, TTuple, TOut>(IParser<TTuple, TOut> constructor, int count) : IParser<IEnumerable<T>, TOut>
{
    public TOut Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return constructor.Parse((TTuple) EnumerableAdapter.Take(source, count));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
    }
}

/// <summary>
/// Take one element from an enumerator and return it.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumerableConstructIdentity<T> : IParser<IEnumerator<T>, T>
{
    public T Parse(IEnumerator<T> input)
    {
        if (!input.MoveNext()) throw new ArgumentOutOfRangeException(nameof(input));
        return input.Current;
    }
}

/// <summary>
/// Take one element from an enumerator and pass it to a constructor parser.
/// </summary>
/// <param name="constructor"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class EnumerableConstructSingle<TIn, TOut>(IParser<TIn, TOut> constructor) : IParser<IEnumerator<TIn>, TOut>
{
    public TOut Parse(IEnumerator<TIn> input)
    {
        if (!input.MoveNext()) throw new ArgumentOutOfRangeException(nameof(input));
        return constructor.Parse(input.Current);
    }
}

/// <summary>
/// Take a fixed number of elements from an enumerator and pass it to a
/// constructor parser as a tuple.
/// </summary>
/// <param name="constructor"></param>
/// <param name="size"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TTuple"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class EnumerableConstructTuple<TIn, TTuple, TOut>(IParser<TTuple, TOut> constructor, int size) : IParser<IEnumerator<TIn>, TOut>
{
    public TOut Parse(IEnumerator<TIn> input)
    {
        return constructor.Parse((TTuple) EnumerableAdapter.Take(input, size));
    }
}

/// <summary>
/// Extension of an enumerable parser that also works on an enumerator.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IEnumerableToTuple<in TIn, out TOut> : IParser<IEnumerable<TIn>, TOut>, IParser<IEnumerator<TIn>, TOut>;

/// <summary>
/// Convert an enumerable to a tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="count1"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T1Out"></typeparam>
public class EnumerableToTuple<T, T1Out>(
    IParser<IEnumerator<T>, T1Out> parser1
) : IEnumerableToTuple<T, ValueTuple<T1Out>>
{
    public ValueTuple<T1Out> Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }
    
    public ValueTuple<T1Out> Parse(IEnumerator<T> source)
    {
        return new ValueTuple<T1Out>(parser1.Parse(source));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2
) : IEnumerableToTuple<T, (T1Out, T2Out)>
{
    public (T1Out, T2Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source)
            );
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3
) : IEnumerableToTuple<T, (T1Out, T2Out, T3Out)>
{
    public (T1Out, T2Out, T3Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out, T3Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source)
            );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out, T4Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3,
    IParser<IEnumerator<T>, T4Out> parser4
) : IEnumerableToTuple<T, (T1Out, T2Out, T3Out, T4Out)>
{
    public (T1Out, T2Out, T3Out, T4Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out, T3Out, T4Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source),
            parser4.Parse(source)
        );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out, T4Out, T5Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3,
    IParser<IEnumerator<T>, T4Out> parser4,
    IParser<IEnumerator<T>, T5Out> parser5
) : IEnumerableToTuple<T, (T1Out, T2Out, T3Out, T4Out, T5Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out, T3Out, T4Out, T5Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source),
            parser4.Parse(source),
            parser5.Parse(source)
        );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
        yield return parser5;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out, T4Out, T5Out, T6Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3,
    IParser<IEnumerator<T>, T4Out> parser4,
    IParser<IEnumerator<T>, T5Out> parser5,
    IParser<IEnumerator<T>, T6Out> parser6
) : IEnumerableToTuple<T, (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source),
            parser4.Parse(source),
            parser5.Parse(source),
            parser6.Parse(source)
        );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
        yield return parser5;
        yield return parser6;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3,
    IParser<IEnumerator<T>, T4Out> parser4,
    IParser<IEnumerator<T>, T5Out> parser5,
    IParser<IEnumerator<T>, T6Out> parser6,
    IParser<IEnumerator<T>, T7Out> parser7
) : IEnumerableToTuple<T, (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out) Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out) Parse(IEnumerator<T> source)
    {
        return (
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source),
            parser4.Parse(source),
            parser5.Parse(source),
            parser6.Parse(source),
            parser7.Parse(source)
        );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
        yield return parser5;
        yield return parser6;
        yield return parser7;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest>(
    IParser<IEnumerator<T>, T1Out> parser1,
    IParser<IEnumerator<T>, T2Out> parser2,
    IParser<IEnumerator<T>, T3Out> parser3,
    IParser<IEnumerator<T>, T4Out> parser4,
    IParser<IEnumerator<T>, T5Out> parser5,
    IParser<IEnumerator<T>, T6Out> parser6,
    IParser<IEnumerator<T>, T7Out> parser7,
    IEnumerableToTuple<T, TRest> parserRest
) : IEnumerableToTuple<T, ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest>>
    where TRest : struct
{
    public ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest> Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }

    public ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest> Parse(IEnumerator<T> source)
    {
        return new ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest>(
            parser1.Parse(source),
            parser2.Parse(source),
            parser3.Parse(source),
            parser4.Parse(source),
            parser5.Parse(source),
            parser6.Parse(source),
            parser7.Parse(source),
            parserRest.Parse(source)
        );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
        yield return parser5;
        yield return parser6;
        yield return parser7;
        yield return parserRest;
    }
}