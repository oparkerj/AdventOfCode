using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Methods for creating a tuple adapter.
/// </summary>
public static class TupleAdapter
{
    /// <summary>
    /// Unwrap a parser returning a 1-tuple.
    /// </summary>
    /// <param name="parser"></param>
    /// <returns></returns>
    public static IParser UnwrapSingle(IParser parser)
    {
        var (input, output) = ParseUtil.GetParserTypesOf(parser);
        Debug.Assert(output.IsTupleType());
        var outputTypes = output.GetTupleTypes();
        Debug.Assert(outputTypes.Length == 1);

        return typeof(TupleUnwrap<,>).NewParserGeneric([input, outputTypes[0]], parser);
    }

    /// <summary>
    /// Get a parser that unwraps a 1-tuple.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IParser UnwrapSingle(Type type)
    {
        return typeof(TupleUnwrap<>).NewParserGeneric([type]);
    }

    /// <summary>
    /// Get a parser that slices a tuple to the given range.
    /// </summary>
    /// <param name="tuple"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static IParser Slice(Type tuple, int start, int length)
    {
        var types = Types.SliceTuple(tuple, start, length);
        var result = Types.CreateTupleType(types);
        return typeof(TupleSlice<,>).NewParserGeneric([tuple, result], start, types);
    }
    
    /// <summary>
    /// Create a tuple adapter.
    /// </summary>
    /// <param name="inputTypes">Input type for each tuple element.</param>
    /// <param name="outputTypes">Output type for each tuple element.</param>
    /// <param name="elementParsers">Parser for each tuple element.</param>
    /// <returns>Tuple adapter.</returns>
    public static IParser Create(ReadOnlySpan<Type> inputTypes, ReadOnlySpan<Type> outputTypes, ReadOnlySpan<IParser> elementParsers)
    {
        Debug.Assert(inputTypes.Length == outputTypes.Length);
        Debug.Assert(inputTypes.Length == elementParsers.Length);

        return CreateInnerType(inputTypes.Length, inputTypes, outputTypes, elementParsers);

        IParser CreateInnerType(int length, ReadOnlySpan<Type> inputs, ReadOnlySpan<Type> outputs, ReadOnlySpan<IParser> parsers)
        {
            return length switch
            {
                1 => typeof(TupleAdapter<,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                2 => typeof(TupleAdapter<,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                3 => typeof(TupleAdapter<,,,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                4 => typeof(TupleAdapter<,,,,,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                5 => typeof(TupleAdapter<,,,,,,,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                6 => typeof(TupleAdapter<,,,,,,,,,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                7 => typeof(TupleAdapter<,,,,,,,,,,,,,>).NewParserGeneric([..inputs, ..outputs], [..parsers]),
                _ => typeof(TupleAdapter<,,,,,,,,,,,,,,,>).NewParserGeneric(
                    [..inputs[..Types.PrimaryTupleSize], Types.CreateTupleType([..inputs[Types.PrimaryTupleSize..]]), ..outputs[..Types.PrimaryTupleSize], Types.CreateTupleType([..outputs[Types.PrimaryTupleSize..]])],
                    [..parsers[..Types.PrimaryTupleSize], CreateInnerType(length - Types.PrimaryTupleSize, inputs[Types.PrimaryTupleSize..], outputs[Types.PrimaryTupleSize..], parsers[Types.PrimaryTupleSize..])])
            };
        }
    }

    /// <summary>
    /// Get a parser that compresses a tuple into a smaller tuple by constructing element types.
    /// </summary>
    /// <param name="source">Input tuple.</param>
    /// <param name="chunks">Tuple chunks.</param>
    /// <returns></returns>
    public static IParser Compress(Type source, TupleChunkParse[] chunks)
    {
        return Compress(source, source.GetTupleTypes(), chunks);
    }

    /// <summary>
    /// Get a parser that compresses a tuple into a smaller tuple by constructing element types.
    /// </summary>
    /// <param name="source">Input tuple.</param>
    /// <param name="sourceTypes">Input tuple types.</param>
    /// <param name="chunks">Tuple chunks.</param>
    /// <returns></returns>
    /// <exception cref="UnreachableException"></exception>
    public static IParser Compress(Type source, Type[] sourceTypes, TupleChunkParse[] chunks)
    {
        return CreateCompress(chunks.Length, 0, sourceTypes, chunks);
        
        IParser CreateCompress(int size, int sectionOffset, ReadOnlySpan<Type> sourceSection, ReadOnlySpan<TupleChunkParse> section)
        {
            const int genericSection = 2;
            const int argSection = 3;
            
            // Set up generic type arguments and constructor arguments
            var sections = Math.Min(section.Length, Types.PrimaryTupleSize);
            var sourceType = Types.CreateTupleType(sourceSection);
            var generic = new Type[sections * genericSection];
            var args = new object?[sections * argSection];
            for (var i = 0; i < sections; i++)
            {
                var (offset, types, parser) = section[i];
                generic[i * genericSection] = Types.CreateTupleType(types);
                generic[i * genericSection + 1] = ParseUtil.GetParserTypesOf(parser).OutputType;

                args[i * argSection] = offset - sectionOffset;
                args[i * argSection + 1] = parser;
                args[i * argSection + 2] = types;
            }

            // Create a "rest" parser
            if (size > Types.PrimaryTupleSize)
            {
                // Where the rest starts
                var remainingStart = section[Types.PrimaryTupleSize].Offset - sectionOffset;
                // Types of the rest section
                var remainingTypes = sourceSection[remainingStart..];
                var remainingInput = Types.CreateTupleType(remainingTypes);
                // Make rest parser
                var rest = CreateCompress(size - Types.PrimaryTupleSize, remainingStart, sourceSection[Types.PrimaryTupleSize..], section[Types.PrimaryTupleSize..]);
                var restOutput = ParseUtil.GetParserTypesOf(rest).OutputType;

                return typeof(TupleCompress<,,,,,,,,,,,,,,,,>).NewParserGeneric(
                    [source, ..generic, remainingInput, restOutput],
                    [..args, remainingStart - sectionOffset, rest, remainingTypes.ToArray()]
                    );
            }
            
            return size switch
            {
                1 => typeof(TupleCompress<,,>).NewParserGeneric([sourceType, ..generic], args),
                2 => typeof(TupleCompress<,,,,>).NewParserGeneric([sourceType, ..generic], args),
                3 => typeof(TupleCompress<,,,,,,>).NewParserGeneric([sourceType, ..generic], args),
                4 => typeof(TupleCompress<,,,,,,,,>).NewParserGeneric([sourceType, ..generic], args),
                5 => typeof(TupleCompress<,,,,,,,,,,>).NewParserGeneric([sourceType, ..generic], args),
                6 => typeof(TupleCompress<,,,,,,,,,,,,>).NewParserGeneric([sourceType, ..generic], args),
                7 => typeof(TupleCompress<,,,,,,,,,,,,,,>).NewParserGeneric([sourceType, ..generic], args),
                _ => throw new UnreachableException()
            };
        }
    }

    /// <summary>
    /// Get a parser that returns the first element of a tuple.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IParser First(Type type)
    {
        Debug.Assert(type.IsTupleType());

        var firstType = type.GetGenericArguments()[0];
        return typeof(TupleFirst<,>).NewParserGeneric([type, firstType]);
    }
}

/// <summary>
/// Represents a slice of a tuple that will be parsed.
/// </summary>
/// <param name="Offset">Offset of the chunk in the source tuple.</param>
/// <param name="Types">Types of the chunk in the source tuple.</param>
/// <param name="Parser">Parser which takes a tuple of <see cref="Types"/> and constructs the resulting type.</param>
public readonly record struct TupleChunkParse(int Offset, Type[] Types, IParser Parser);

/// <summary>
/// Unwrap a 1-tuple.
/// </summary>
/// <param name="parser"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class TupleUnwrap<TIn, TOut>(IParser<TIn, ValueTuple<TOut>> parser) : IParser<TIn, TOut>
{
    public TOut Parse(TIn input) => parser.Parse(input).Item1;
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser;
    }
}

/// <summary>
/// Unwrap a 1-tuple.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TupleUnwrap<T> : IParser<ValueTuple<T>, T>
{
    public T Parse(ValueTuple<T> input) => input.Item1;
}

/// <summary>
/// Get a slice of a tuple.
/// </summary>
/// <param name="offset">Slice start index.</param>
/// <param name="types">Slice types.</param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class TupleSlice<TIn, TOut>(int offset, Type[] types) : IParser<TIn, TOut>
    where TIn : ITuple
{
    public TOut Parse(TIn input)
    {
        return (TOut) Types.SliceTuple(input, types, offset);
    }
}

/// <summary>
/// Get the first element of a tuple.
/// </summary>
/// <typeparam name="TTuple"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class TupleFirst<TTuple, TOut> : IParser<TTuple, TOut>
    where TTuple : ITuple
{
    public TOut Parse(TTuple input)
    {
        if (input is [TOut first, ..]) return first;
        throw new UnreachableException();
    }
}

/// <summary>
/// Apply a parse to each element of a 1-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TO1"></typeparam>
public class TupleAdapter<TI1, TO1>(
    IParser<TI1, TO1> parser1
) 
    : IParser<ValueTuple<TI1>, ValueTuple<TO1>>
{
    public ValueTuple<TO1> Parse(ValueTuple<TI1> input)
    {
        return new ValueTuple<TO1>(parser1.Parse(input.Item1));
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
    }
}

/// <summary>
/// Apply a parse to each element of a 2-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
public class TupleAdapter<TI1, TI2, TO1, TO2>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2
) 
    : IParser<(TI1, TI2), (TO1, TO2)>
{
    public (TO1, TO2) Parse((TI1, TI2) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2));
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
    }
}

/// <summary>
/// Apply a parse to each element of a 3-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TO1, TO2, TO3>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3
) 
    : IParser<(TI1, TI2, TI3), (TO1, TO2, TO3)>
{
    public (TO1, TO2, TO3) Parse((TI1, TI2, TI3) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3));
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
    }
}

/// <summary>
/// Apply a parse to each element of a 4-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <param name="parser4"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TI4"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
/// <typeparam name="TO4"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TI4, TO1, TO2, TO3, TO4>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3,
    IParser<TI4, TO4> parser4
) 
    : IParser<(TI1, TI2, TI3, TI4), (TO1, TO2, TO3, TO4)>
{
    public (TO1, TO2, TO3, TO4) Parse((TI1, TI2, TI3, TI4) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3),
            parser4.Parse(input.Item4));
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
    }
}

/// <summary>
/// Apply a parse to each element of a 5-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <param name="parser4"></param>
/// <param name="parser5"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TI4"></typeparam>
/// <typeparam name="TI5"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
/// <typeparam name="TO4"></typeparam>
/// <typeparam name="TO5"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TI4, TI5, TO1, TO2, TO3, TO4, TO5>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3,
    IParser<TI4, TO4> parser4,
    IParser<TI5, TO5> parser5
) 
    : IParser<(TI1, TI2, TI3, TI4, TI5), (TO1, TO2, TO3, TO4, TO5)>
{
    public (TO1, TO2, TO3, TO4, TO5) Parse((TI1, TI2, TI3, TI4, TI5) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3),
            parser4.Parse(input.Item4),
            parser5.Parse(input.Item5));
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

/// <summary>
/// Apply a parse to each element of a 6-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <param name="parser4"></param>
/// <param name="parser5"></param>
/// <param name="parser6"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TI4"></typeparam>
/// <typeparam name="TI5"></typeparam>
/// <typeparam name="TI6"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
/// <typeparam name="TO4"></typeparam>
/// <typeparam name="TO5"></typeparam>
/// <typeparam name="TO6"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TI4, TI5, TI6, TO1, TO2, TO3, TO4, TO5, TO6>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3,
    IParser<TI4, TO4> parser4,
    IParser<TI5, TO5> parser5,
    IParser<TI6, TO6> parser6
) 
    : IParser<(TI1, TI2, TI3, TI4, TI5, TI6), (TO1, TO2, TO3, TO4, TO5, TO6)>
{
    public (TO1, TO2, TO3, TO4, TO5, TO6) Parse((TI1, TI2, TI3, TI4, TI5, TI6) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3),
            parser4.Parse(input.Item4),
            parser5.Parse(input.Item5),
            parser6.Parse(input.Item6));
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

/// <summary>
/// Apply a parse to each element of a 7-tuple.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <param name="parser4"></param>
/// <param name="parser5"></param>
/// <param name="parser6"></param>
/// <param name="parser7"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TI4"></typeparam>
/// <typeparam name="TI5"></typeparam>
/// <typeparam name="TI6"></typeparam>
/// <typeparam name="TI7"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
/// <typeparam name="TO4"></typeparam>
/// <typeparam name="TO5"></typeparam>
/// <typeparam name="TO6"></typeparam>
/// <typeparam name="TO7"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TO1, TO2, TO3, TO4, TO5, TO6, TO7>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3,
    IParser<TI4, TO4> parser4,
    IParser<TI5, TO5> parser5,
    IParser<TI6, TO6> parser6,
    IParser<TI7, TO7> parser7
) 
    : IParser<(TI1, TI2, TI3, TI4, TI5, TI6, TI7), (TO1, TO2, TO3, TO4, TO5, TO6, TO7)>
{
    public (TO1, TO2, TO3, TO4, TO5, TO6, TO7) Parse((TI1, TI2, TI3, TI4, TI5, TI6, TI7) input)
    {
        return (
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3),
            parser4.Parse(input.Item4),
            parser5.Parse(input.Item5),
            parser6.Parse(input.Item6),
            parser7.Parse(input.Item7));
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

/// <summary>
/// Apply a parse to each element of a tuple of size 8 or more.
/// </summary>
/// <param name="parser1"></param>
/// <param name="parser2"></param>
/// <param name="parser3"></param>
/// <param name="parser4"></param>
/// <param name="parser5"></param>
/// <param name="parser6"></param>
/// <param name="parser7"></param>
/// <param name="parserRest"></param>
/// <typeparam name="TI1"></typeparam>
/// <typeparam name="TI2"></typeparam>
/// <typeparam name="TI3"></typeparam>
/// <typeparam name="TI4"></typeparam>
/// <typeparam name="TI5"></typeparam>
/// <typeparam name="TI6"></typeparam>
/// <typeparam name="TI7"></typeparam>
/// <typeparam name="TInRest"></typeparam>
/// <typeparam name="TO1"></typeparam>
/// <typeparam name="TO2"></typeparam>
/// <typeparam name="TO3"></typeparam>
/// <typeparam name="TO4"></typeparam>
/// <typeparam name="TO5"></typeparam>
/// <typeparam name="TO6"></typeparam>
/// <typeparam name="TO7"></typeparam>
/// <typeparam name="TOutRest"></typeparam>
public class TupleAdapter<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TInRest, TO1, TO2, TO3, TO4, TO5, TO6, TO7, TOutRest>(
    IParser<TI1, TO1> parser1,
    IParser<TI2, TO2> parser2,
    IParser<TI3, TO3> parser3,
    IParser<TI4, TO4> parser4,
    IParser<TI5, TO5> parser5,
    IParser<TI6, TO6> parser6,
    IParser<TI7, TO7> parser7,
    IParser<TInRest, TOutRest> parserRest
) 
    : IParser<ValueTuple<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TInRest>, ValueTuple<TO1, TO2, TO3, TO4, TO5, TO6, TO7, TOutRest>>
    where TInRest : struct
    where TOutRest : struct
{
    public ValueTuple<TO1, TO2, TO3, TO4, TO5, TO6, TO7, TOutRest> Parse(ValueTuple<TI1, TI2, TI3, TI4, TI5, TI6, TI7, TInRest> input)
    {
        return new ValueTuple<TO1, TO2, TO3, TO4, TO5, TO6, TO7, TOutRest>(
            parser1.Parse(input.Item1),
            parser2.Parse(input.Item2),
            parser3.Parse(input.Item3),
            parser4.Parse(input.Item4),
            parser5.Parse(input.Item5),
            parser6.Parse(input.Item6),
            parser7.Parse(input.Item7),
            parserRest.Parse(input.Rest));
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

/// <summary>
/// Compress a tuple into 1-tuple.
/// </summary>
/// <param name="offset1"></param>
/// <param name="parser1"></param>
/// <param name="types1"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TChunk1"></typeparam>
/// <typeparam name="T1"></typeparam>
public class TupleCompress<
    TIn, TChunk1, T1
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1
    ) : IParser<TIn, ValueTuple<T1>>
    where TIn : ITuple
{
    public ValueTuple<T1> Parse(TIn input)
    {
        return new ValueTuple<T1>(parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1)));
    }

    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
    }
}

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2
    ) : IParser<TIn, (T1, T2)>
    where TIn : ITuple
{
    public (T1, T2) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        return (e1, e2);
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
    }
}

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3
) : IParser<TIn, (T1, T2, T3)>
    where TIn : ITuple
{
    public (T1, T2, T3) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        return (e1, e2, e3);
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
    }
}

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3,
    TChunk4, T4
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3,
    int offset4, IParser<TChunk4, T4> parser4, Type[] types4
) : IParser<TIn, (T1, T2, T3, T4)>
    where TIn : ITuple
{
    public (T1, T2, T3, T4) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        var e4 = parser4.Parse((TChunk4) Types.SliceTuple(input, types4, offset4));
        return (e1, e2, e3, e4);
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser1;
        yield return parser2;
        yield return parser3;
        yield return parser4;
    }
}

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3,
    TChunk4, T4,
    TChunk5, T5
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3,
    int offset4, IParser<TChunk4, T4> parser4, Type[] types4,
    int offset5, IParser<TChunk5, T5> parser5, Type[] types5
) : IParser<TIn, (T1, T2, T3, T4, T5)>
    where TIn : ITuple
{
    public (T1, T2, T3, T4, T5) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        var e4 = parser4.Parse((TChunk4) Types.SliceTuple(input, types4, offset4));
        var e5 = parser5.Parse((TChunk5) Types.SliceTuple(input, types5, offset5));
        return (e1, e2, e3, e4, e5);
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

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3,
    TChunk4, T4,
    TChunk5, T5,
    TChunk6, T6
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3,
    int offset4, IParser<TChunk4, T4> parser4, Type[] types4,
    int offset5, IParser<TChunk5, T5> parser5, Type[] types5,
    int offset6, IParser<TChunk6, T6> parser6, Type[] types6
) : IParser<TIn, (T1, T2, T3, T4, T5, T6)>
    where TIn : ITuple
{
    public (T1, T2, T3, T4, T5, T6) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        var e4 = parser4.Parse((TChunk4) Types.SliceTuple(input, types4, offset4));
        var e5 = parser5.Parse((TChunk5) Types.SliceTuple(input, types5, offset5));
        var e6 = parser6.Parse((TChunk6) Types.SliceTuple(input, types6, offset6));
        return (e1, e2, e3, e4, e5, e6);
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

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3,
    TChunk4, T4,
    TChunk5, T5,
    TChunk6, T6,
    TChunk7, T7
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3,
    int offset4, IParser<TChunk4, T4> parser4, Type[] types4,
    int offset5, IParser<TChunk5, T5> parser5, Type[] types5,
    int offset6, IParser<TChunk6, T6> parser6, Type[] types6,
    int offset7, IParser<TChunk7, T7> parser7, Type[] types7
) : IParser<TIn, (T1, T2, T3, T4, T5, T6, T7)>
    where TIn : ITuple
{
    public (T1, T2, T3, T4, T5, T6, T7) Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        var e4 = parser4.Parse((TChunk4) Types.SliceTuple(input, types4, offset4));
        var e5 = parser5.Parse((TChunk5) Types.SliceTuple(input, types5, offset5));
        var e6 = parser6.Parse((TChunk6) Types.SliceTuple(input, types6, offset6));
        var e7 = parser7.Parse((TChunk7) Types.SliceTuple(input, types7, offset7));
        return (e1, e2, e3, e4, e5, e6, e7);
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

public class TupleCompress<
    TIn,
    TChunk1, T1,
    TChunk2, T2,
    TChunk3, T3,
    TChunk4, T4,
    TChunk5, T5,
    TChunk6, T6,
    TChunk7, T7,
    TChunkRest, TRest
>(
    int offset1, IParser<TChunk1, T1> parser1, Type[] types1,
    int offset2, IParser<TChunk2, T2> parser2, Type[] types2,
    int offset3, IParser<TChunk3, T3> parser3, Type[] types3,
    int offset4, IParser<TChunk4, T4> parser4, Type[] types4,
    int offset5, IParser<TChunk5, T5> parser5, Type[] types5,
    int offset6, IParser<TChunk6, T6> parser6, Type[] types6,
    int offset7, IParser<TChunk7, T7> parser7, Type[] types7,
    int offsetRest, IParser<TChunkRest, TRest> parserRest, Type[] typesRest
) : IParser<TIn, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
    where TIn : ITuple
    where TRest : struct
{
    public ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Parse(TIn input)
    {
        var e1 = parser1.Parse((TChunk1) Types.SliceTuple(input, types1, offset1));
        var e2 = parser2.Parse((TChunk2) Types.SliceTuple(input, types2, offset2));
        var e3 = parser3.Parse((TChunk3) Types.SliceTuple(input, types3, offset3));
        var e4 = parser4.Parse((TChunk4) Types.SliceTuple(input, types4, offset4));
        var e5 = parser5.Parse((TChunk5) Types.SliceTuple(input, types5, offset5));
        var e6 = parser6.Parse((TChunk6) Types.SliceTuple(input, types6, offset6));
        var e7 = parser7.Parse((TChunk7) Types.SliceTuple(input, types7, offset7));
        var rest = parserRest.Parse((TChunkRest) Types.SliceTuple(input, typesRest, offsetRest));
        return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(e1, e2, e3, e4, e5, e6, e7, rest);
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