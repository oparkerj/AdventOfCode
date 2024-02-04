using System.Diagnostics;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Methods for creating a tuple adapter.
/// </summary>
public static class TupleAdapter
{
    /// <summary>
    /// Create a tuple adapter.
    /// </summary>
    /// <param name="inputTypes">Input type for each tuple element.</param>
    /// <param name="outputTypes">Output type for each tuple element.</param>
    /// <param name="elementParsers">Parser for each tuple element.</param>
    /// <returns>Tuple adapter.</returns>
    public static IParser Create(Type[] inputTypes, Type[] outputTypes, IParser[] elementParsers)
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
                    [..inputs[..7], Types.CreateTupleType([..inputs[7..]]), ..outputs[..7], Types.CreateTupleType([..outputs[7..]])],
                    [..parsers[..7], CreateInnerType(length - 7, inputs[7..], outputs[7..], parsers[7..])])
            };
        }
    }
    
    public static IParser UnwrapSingle(IParser parser)
    {
        var (output, input) = ParseUtil.GetParserTypesOf(parser);
        Debug.Assert(input.IsTupleType());
        var outputTypes = input.GetTupleTypes();
        Debug.Assert(outputTypes.Length == 1);

        return typeof(TupleUnwrap<,>).NewParserGeneric([output, outputTypes[0]], parser);
    }
}

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