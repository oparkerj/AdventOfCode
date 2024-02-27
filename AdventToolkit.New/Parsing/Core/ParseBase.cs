using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Core;

/// <summary>
/// A parse base is a "root" parser, which stores a context that will be
/// used during parsing.
/// </summary>
/// <param name="context"></param>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public abstract class ParseBase<TIn, TOut>(IReadOnlyParseContext context)
    : IParser<TIn, TOut>
{
    /// <summary>
    /// Current parse context.
    /// </summary>
    public IReadOnlyParseContext Context { get; set; } = context;

    public abstract TOut Parse(TIn input);

    public abstract IEnumerable<IParser> GetChildren();
}