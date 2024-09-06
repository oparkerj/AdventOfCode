using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Core;

/// <summary>
/// A parse base is a "root" parser, which stores a context that will be
/// used during parsing.
/// </summary>
/// <param name="context"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public abstract class ParseBase<TIn, TOut>(IParseContext context)
    : IParser<TIn, TOut>
{
    /// <summary>
    /// Current parse context.
    /// </summary>
    public IParseContext Context { get; set; } = context;

    public abstract TOut Parse(TIn input);

    public abstract IEnumerable<IParser> GetChildren();
}