using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Selection helper methods.
/// </summary>
public static class SelectAdapter
{
    /// <summary>
    /// Wrap a parser in a select adapter.
    /// </summary>
    /// <param name="parser"></param>
    /// <returns></returns>
    public static IParser Create(IParser parser)
    {
        var (input, output) = ParseUtil.GetParserTypesOf(parser);
        return typeof(SelectAdapter<,>).NewParserGeneric([input, output], parser);
    }
}

/// <summary>
/// Parser that takes a sequence of items and applies the inner parser
/// to each element.
/// </summary>
/// <param name="parser">Element parser.</param>
/// <typeparam name="TIn">Element input type.</typeparam>
/// <typeparam name="TOut">Element output type.</typeparam>
public class SelectAdapter<TIn, TOut>(IParser<TIn, TOut> parser) :
    IParser<IEnumerable<TIn>, IEnumerable<TOut>>,
    IParseJoin
{
    public IEnumerable<TOut> Parse(IEnumerable<TIn> input)
    {
        foreach (var t in input)
        {
            yield return parser.Parse(t);
        }
    }
    
    public IParser InnerJoin(IParser other, int targetLevel, IParseContext context)
    {
        if (targetLevel > 0)
        {
            var inner = ParseJoin.InnerJoin(parser, other, targetLevel - 1, context);
            return SelectAdapter.Create(inner);
        }
        
        return SelectAdapter.Create(ParseJoin.Create(parser, other));
    }
    
    public IEnumerable<IParser> GetChildren()
    {
        yield return parser;
    }
}