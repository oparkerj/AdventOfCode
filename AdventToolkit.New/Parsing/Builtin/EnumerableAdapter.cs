using System.Diagnostics;
using AdventToolkit.New.Data;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;
using AdventToolkit.New.Util;

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
    /// <typeparam name="TTuple"></typeparam>
    /// <returns>Tuple of items.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static TTuple Take<T, TTuple>(IEnumerator<T> source, int count)
    {
        using var arr = Arr<object?>.Get(count);

        for (var i = 0; i < count; i++)
        {
            if (!source.MoveNext()) throw new ArgumentOutOfRangeException(nameof(source));
            arr[i] = source.Current;
        }

        return (TTuple) Types.CreateTupleFrom(typeof(TTuple), arr);
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
        Debug.Assert(input.IsTupleType());
        var size = input.GetTupleSize();
        return typeof(EnumerableToValue<,,>).NewParserGeneric([elements, input, output], constructor, size);
    }

    /// <summary>
    /// Get a parser that converts a sequence to an inner element which gets collected
    /// to an output container.
    /// </summary>
    /// <param name="elements">Element type of the input sequence.</param>
    /// <param name="constructor">Container constructor.</param>
    /// <param name="innerConstructor">Element constructor.</param>
    /// <returns></returns>
    public static IParser ConstructInner(Type elements, IParser constructor, IParser innerConstructor)
    {
        var output = ParseUtil.GetParserTypesOf(constructor).OutputType;
        var (innerInput, innerOutput) = ParseUtil.GetParserTypesOf(innerConstructor);
        Debug.Assert(innerInput.IsTupleType());
        var size = innerInput.GetTupleSize();

        return typeof(EnumerableConstructInner<,,,>).NewParserGeneric(
            [elements, innerInput, innerOutput, output],
            innerConstructor, constructor, size);
    }
    
    /// <summary>
    /// Get a parser that converts a sequence to an inner element which gets collected
    /// to an output container.
    /// </summary>
    /// <param name="elements">Element type of the input sequence.</param>
    /// <param name="constructor">Container constructor.</param>
    /// <param name="innerConstructor">Element constructor.</param>
    /// <returns></returns>
    public static IParser ConstructInnerTuple(Type elements, IParser constructor, IParser innerConstructor)
    {
        Debug.Assert(innerConstructor.TryGetTypeArgumentsOf(typeof(IEnumerableParser<,>), out _));
        
        var output = ParseUtil.GetParserTypesOf(constructor).OutputType;
        var innerOutput = ParseUtil.GetParserTypesOf(innerConstructor).OutputType;
        Debug.Assert(innerOutput.IsTupleType());
        var size = innerOutput.GetTupleSize();

        return typeof(EnumerableConstructInnerTuple<,,>).NewParserGeneric(
            [elements, innerOutput, output],
            innerConstructor, constructor, size);
    }

    /// <summary>
    /// Get a parser that converts an enumerable to a tuple.
    /// Elements are taken from the enumerable to construct each element
    /// of the tuple.
    /// </summary>
    /// <param name="elements">Enumerable inner type.</param>
    /// <param name="parsers">Instances of <see cref="IEnumerableParser{TIn,TOut}"/></param>
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
        return constructor.Parse(EnumerableAdapter.Take<T, TTuple>(source, count));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
    }
}

/// <summary>
/// Extension of an enumerable parser that also works on an enumerator.
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public interface IEnumerableParser<TIn, out TOut> : IParser<IEnumerable<TIn>, TOut>, IParser<BufferedEnumerator<TIn>, TOut>
{
    TOut IParser<IEnumerable<TIn>, TOut>.Parse(IEnumerable<TIn> input)
    {
        using var e = new BufferedEnumerator<TIn>(input);
        return Parse(e);
    }
}

/// <summary>
/// Take one element from an enumerator and return it.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumerableConstructIdentity<T> : IEnumerableParser<T, T>
{
    public T Parse(BufferedEnumerator<T> input)
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
public class EnumerableConstructSingle<TIn, TOut>(IParser<TIn, TOut> constructor) : IEnumerableParser<TIn, TOut>
{
    public TOut Parse(BufferedEnumerator<TIn> input)
    {
        if (!input.MoveNext()) throw new ArgumentOutOfRangeException(nameof(input));
        return constructor.Parse(input.Current);
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
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
public class EnumerableConstructTuple<TIn, TTuple, TOut>(IParser<TTuple, TOut> constructor, int size) : IEnumerableParser<TIn, TOut>
{
    public TOut Parse(BufferedEnumerator<TIn> input)
    {
        return constructor.Parse(EnumerableAdapter.Take<TIn, TTuple>(input, size));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
    }
}

/// <summary>
/// Construct an inner element and collect it to a result container.
/// </summary>
/// <param name="constructor"></param>
/// <param name="collector"></param>
/// <param name="size"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TTuple"></typeparam>
/// <typeparam name="TInner"></typeparam>
/// <typeparam name="TCollect"></typeparam>
public class EnumerableConstructInner<TIn, TTuple, TInner, TCollect>(
    IParser<TTuple, TInner> constructor,
    IParser<IEnumerable<TInner>, TCollect> collector,
    int size
    ) : IEnumerableParser<TIn, TCollect>
{
    private IEnumerable<TInner> Construct(BufferedEnumerator<TIn> inner)
    {
        using Arr<object?> buffer = new(size);

        // Create items as long as we can buffer the required number of arguments
        while (inner.TryBuffer(size))
        {
            for (var i = 0; i < size; i++)
            {
                buffer[i] = inner.Next();
            }
            var tuple = (TTuple) Types.CreateTupleFrom(typeof(TTuple), buffer);
            yield return constructor.Parse(tuple);
        }
    }
    
    public TCollect Parse(BufferedEnumerator<TIn> input) => collector.Parse(Construct(input));

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
        yield return collector;
    }
}

/// <summary>
/// Construct a container of tuples.
/// </summary>
/// <param name="constructor">Tuple constructor.</param>
/// <param name="collector">Container constructor.</param>
/// <param name="size">Tuple size.</param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TTuple"></typeparam>
/// <typeparam name="TCollect"></typeparam>
public class EnumerableConstructInnerTuple<TIn, TTuple, TCollect>(
    IEnumerableParser<TIn, TTuple> constructor,
    IParser<IEnumerable<TTuple>, TCollect> collector,
    int size
) : IEnumerableParser<TIn, TCollect>
{
    private IEnumerable<TTuple> Construct(BufferedEnumerator<TIn> inner)
    {
        // Create tuples as long as we can buffer the required number of arguments
        while (inner.TryBuffer(size))
        {
            yield return constructor.Parse(inner);
        }
    }
    
    public TCollect Parse(BufferedEnumerator<TIn> input) => collector.Parse(Construct(input));

    public IEnumerable<IParser> GetChildren()
    {
        yield return constructor;
        yield return collector;
    }
}

/// <summary>
/// Convert an enumerable to a tuple.
/// </summary>
/// <param name="parser1"></param>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T1Out"></typeparam>
public class EnumerableToTuple<T, T1Out>(
    IEnumerableParser<T, T1Out> parser1
) : IEnumerableParser<T, ValueTuple<T1Out>>
{
    public ValueTuple<T1Out> Parse(BufferedEnumerator<T> source)
    {
        return new ValueTuple<T1Out>(parser1.Parse(source));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
    }
}

public class EnumerableToTuple<T, T1Out, T2Out>(
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2
) : IEnumerableParser<T, (T1Out, T2Out)>
{
    public (T1Out, T2Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3
) : IEnumerableParser<T, (T1Out, T2Out, T3Out)>
{
    public (T1Out, T2Out, T3Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3,
    IEnumerableParser<T, T4Out> parser4
) : IEnumerableParser<T, (T1Out, T2Out, T3Out, T4Out)>
{
    public (T1Out, T2Out, T3Out, T4Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3,
    IEnumerableParser<T, T4Out> parser4,
    IEnumerableParser<T, T5Out> parser5
) : IEnumerableParser<T, (T1Out, T2Out, T3Out, T4Out, T5Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3,
    IEnumerableParser<T, T4Out> parser4,
    IEnumerableParser<T, T5Out> parser5,
    IEnumerableParser<T, T6Out> parser6
) : IEnumerableParser<T, (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3,
    IEnumerableParser<T, T4Out> parser4,
    IEnumerableParser<T, T5Out> parser5,
    IEnumerableParser<T, T6Out> parser6,
    IEnumerableParser<T, T7Out> parser7
) : IEnumerableParser<T, (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out)>
{
    public (T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out) Parse(BufferedEnumerator<T> source)
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
    IEnumerableParser<T, T1Out> parser1,
    IEnumerableParser<T, T2Out> parser2,
    IEnumerableParser<T, T3Out> parser3,
    IEnumerableParser<T, T4Out> parser4,
    IEnumerableParser<T, T5Out> parser5,
    IEnumerableParser<T, T6Out> parser6,
    IEnumerableParser<T, T7Out> parser7,
    IEnumerableParser<T, TRest> parserRest
) : IEnumerableParser<T, ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest>>
    where TRest : struct
{
    public ValueTuple<T1Out, T2Out, T3Out, T4Out, T5Out, T6Out, T7Out, TRest> Parse(BufferedEnumerator<T> source)
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