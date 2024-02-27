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
    /// <param name="parsers"></param>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    public static IParser ToTuple(Type elements, ReadOnlySpan<IParser> parsers)
    {
        const int genericSize = 2;
        const int argSize = 2;

        // Set up generic types and constructor args
        var count = Math.Min(parsers.Length, Types.PrimaryTupleSize);
        var generic = new Type[parsers.Length * genericSize];
        var args = new object?[parsers.Length * argSize];
        for (var i = 0; i < count; i++)
        {
            var (input, output) = ParseUtil.GetParserTypesOf(parsers[i]);
            generic[i * genericSize] = input;
            generic[i * genericSize + 1] = output;

            args[i * argSize] = parsers[i];
            args[i * argSize + 1] = input.GetTupleSize();
        }

        if (parsers.Length > Types.PrimaryTupleSize)
        {
            var rest = ToTuple(elements, parsers[Types.PrimaryTupleSize..]);
            var output = ParseUtil.GetParserTypesOf(rest).OutputType;
            return typeof(EnumerableToTuple<,,,,,,,,,,,,,,,>).NewParserGeneric([elements, ..generic, output], [..args, rest]);
        }

        return count switch
        {
            1 => typeof(EnumerableToTuple<,,>).NewParserGeneric([elements, ..generic], args),
            2 => typeof(EnumerableToTuple<,,,,>).NewParserGeneric([elements, ..generic], args),
            3 => typeof(EnumerableToTuple<,,,,,,>).NewParserGeneric([elements, ..generic], args),
            4 => typeof(EnumerableToTuple<,,,,,,,,>).NewParserGeneric([elements, ..generic], args),
            5 => typeof(EnumerableToTuple<,,,,,,,,,,>).NewParserGeneric([elements, ..generic], args),
            6 => typeof(EnumerableToTuple<,,,,,,,,,,,,>).NewParserGeneric([elements, ..generic], args),
            7 => typeof(EnumerableToTuple<,,,,,,,,,,,,,,>).NewParserGeneric([elements, ..generic], args),
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
/// Extension of an enumerable parser that also works on an enumerator.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IEnumerableToTuple<in TIn, out TOut> : IParser<IEnumerable<TIn>, TOut>
{
    TOut Parse(IEnumerator<TIn> source);
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
/// Convert an enumerable to a tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="count1"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T1In"></typeparam>
/// <typeparam name="T1Out"></typeparam>
public class EnumerableToTuple<T,
    T1In, T1Out
>(
    IParser<T1In, T1Out> parser1, int count1
) : IEnumerableToTuple<T, ValueTuple<T1Out>>
{
    public ValueTuple<T1Out> Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return Parse(source);
    }
    
    public ValueTuple<T1Out> Parse(IEnumerator<T> source)
    {
        return new ValueTuple<T1Out>(parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
    }
}

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2))
            );
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
    }
}

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3))
            );
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
    }
}

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out,
    T4In, T4Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3,
    IParser<T4In, T4Out> parser4, int count4
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3)),
            parser4.Parse((T4In) EnumerableAdapter.Take(source, count4))
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

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out,
    T4In, T4Out,
    T5In, T5Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3,
    IParser<T4In, T4Out> parser4, int count4,
    IParser<T5In, T5Out> parser5, int count5
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3)),
            parser4.Parse((T4In) EnumerableAdapter.Take(source, count4)),
            parser5.Parse((T5In) EnumerableAdapter.Take(source, count5))
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

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out,
    T4In, T4Out,
    T5In, T5Out,
    T6In, T6Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3,
    IParser<T4In, T4Out> parser4, int count4,
    IParser<T5In, T5Out> parser5, int count5,
    IParser<T6In, T6Out> parser6, int count6
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3)),
            parser4.Parse((T4In) EnumerableAdapter.Take(source, count4)),
            parser5.Parse((T5In) EnumerableAdapter.Take(source, count5)),
            parser6.Parse((T6In) EnumerableAdapter.Take(source, count6))
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

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out,
    T4In, T4Out,
    T5In, T5Out,
    T6In, T6Out,
    T7In, T7Out
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3,
    IParser<T4In, T4Out> parser4, int count4,
    IParser<T5In, T5Out> parser5, int count5,
    IParser<T6In, T6Out> parser6, int count6,
    IParser<T7In, T7Out> parser7, int count7
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3)),
            parser4.Parse((T4In) EnumerableAdapter.Take(source, count4)),
            parser5.Parse((T5In) EnumerableAdapter.Take(source, count5)),
            parser6.Parse((T6In) EnumerableAdapter.Take(source, count6)),
            parser7.Parse((T7In) EnumerableAdapter.Take(source, count7))
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

public class EnumerableToTuple<T,
    T1In, T1Out,
    T2In, T2Out,
    T3In, T3Out,
    T4In, T4Out,
    T5In, T5Out,
    T6In, T6Out,
    T7In, T7Out,
    TRest
>(
    IParser<T1In, T1Out> parser1, int count1,
    IParser<T2In, T2Out> parser2, int count2,
    IParser<T3In, T3Out> parser3, int count3,
    IParser<T4In, T4Out> parser4, int count4,
    IParser<T5In, T5Out> parser5, int count5,
    IParser<T6In, T6Out> parser6, int count6,
    IParser<T7In, T7Out> parser7, int count7,
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
            parser1.Parse((T1In) EnumerableAdapter.Take(source, count1)),
            parser2.Parse((T2In) EnumerableAdapter.Take(source, count2)),
            parser3.Parse((T3In) EnumerableAdapter.Take(source, count3)),
            parser4.Parse((T4In) EnumerableAdapter.Take(source, count4)),
            parser5.Parse((T5In) EnumerableAdapter.Take(source, count5)),
            parser6.Parse((T6In) EnumerableAdapter.Take(source, count6)),
            parser7.Parse((T7In) EnumerableAdapter.Take(source, count7)),
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