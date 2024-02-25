using System.Diagnostics;
using AdventToolkit.New.Data;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Builtin;

public static class EnumerableAdapter
{
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

    public static IParser First(Type type)
    {
        return typeof(EnumerableFirst<>).NewParserGeneric([type]);
    }
    
    public static IParser Single(Type elements, IParser constructor)
    {
        var (input, output) = ParseUtil.GetParserTypesOf(constructor);
        var size = input.GetTupleSize();
        return typeof(EnumerableToValue<,,>).NewParserGeneric([elements, input, output], constructor, size);
    }

    public static IParser Create(Type elements, ReadOnlySpan<IParser> parsers)
    {
        const int genericSize = 2;
        const int argSize = 2;

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
            var rest = Create(elements, parsers[Types.PrimaryTupleSize..]);
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

public class EnumerableFirst<T> : IParser<IEnumerable<T>, T>
{
    public T Parse(IEnumerable<T> input) => input.First();
}

public interface IEnumerableToTuple<in TIn, out TOut> : IParser<IEnumerable<TIn>, TOut>
{
    TOut Parse(IEnumerator<TIn> source);
}

public class EnumerableToValue<T, TTuple, TOut>(IParser<TTuple, TOut> constructor, int count) : IParser<IEnumerable<T>, TOut>
{
    public TOut Parse(IEnumerable<T> input)
    {
        using var source = input.GetEnumerator();
        return constructor.Parse((TTuple) EnumerableAdapter.Take(source, count));
    }
}

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
}