using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing;

/// <summary>
/// Methods for joining parsers together.
/// </summary>
public static class ParseJoin
{
    /// <summary>
    /// Create an instance of a parse join.
    /// </summary>
    /// <param name="inType">Input type.</param>
    /// <param name="joinType">Type that connects first output and second input.</param>
    /// <param name="outType">Output type.</param>
    /// <param name="first">First parser.</param>
    /// <param name="second">Second parser.</param>
    /// <returns>Joined parser.</returns>
    private static IParser Create(Type inType, Type joinType, Type outType, IParser first, IParser second)
    {
        return typeof(ParseJoin<,,>).NewParserGeneric([inType, joinType, outType], first, second);
    }
    
    /// <summary>
    /// Join two parsers.
    /// </summary>
    /// <param name="first">First parser.</param>
    /// <param name="second">Second parser.</param>
    /// <returns>Joined parser.</returns>
    public static IParser Create(IParser first, IParser second)
    {
        var inType = ParseUtil.GetParserTypesOf(first).InputType;
        var (joinType, outType) = ParseUtil.GetParserTypesOf(second);

        return Create(inType, joinType, outType, first, second);
    }

    /// <summary>
    /// Join two parsers, trying to adapt the output of the first parser
    /// to the input of the second parser.
    /// </summary>
    /// <param name="first">First parser.</param>
    /// <param name="second">Second parser.</param>
    /// <param name="context">Parse context.</param>
    /// <returns>Joined and possibly adapted parser.</returns>
    public static IParser CreateAdapt(IParser first, IParser second, IReadOnlyParseContext context)
    {
        var inType = ParseUtil.GetParserTypesOf(first).InputType;
        var (secondInput, outType) = ParseUtil.GetParserTypesOf(second);
        var adapted = ParseAdapt.Adapt(first, secondInput, context);
        
        return Create(inType, secondInput, outType, adapted, second);
    }

    /// <summary>
    /// Inner join the parser to the current parser.
    ///
    /// An inner join inserts the parser at the requested nest level.
    /// </summary>
    /// <param name="current">Current parser.</param>
    /// <param name="join">Inner parser.</param>
    /// <param name="level">Target nest level.</param>
    /// <param name="context">Parse context.</param>
    /// <returns>Joined parser.</returns>
    public static IParser InnerJoin(IParser current, IParser join, int level, IReadOnlyParseContext context)
    {
        if (current is IParseJoin parseJoin) return parseJoin.InnerJoin(join, level, context);
        return Create(current, join.AddLevels(level));
    }
}

public class ParseJoin<TIn, TJoin, TOut> : IParser<TIn, TOut>
{
    public IParser<TIn, TJoin> First { get; }
    
    public IParser<TJoin, TOut> Second { get; }

    public ParseJoin(IParser<TIn, TJoin> first, IParser<TJoin, TOut> second)
    {
        First = first;
        Second = second;
    }

    public TOut Parse(TIn input) => Second.Parse(First.Parse(input));

    public IEnumerable<IParser> GetChildren()
    {
        yield return First;
        yield return Second;
    }
}