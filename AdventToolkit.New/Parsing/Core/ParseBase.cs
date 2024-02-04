using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Core;

public abstract class ParseBase<TIn, TOut>(IReadOnlyParseContext context)
    : IParser<TIn, TOut>
{
    public IReadOnlyParseContext Context { get; set; } = context;

    public abstract TOut Parse(TIn input);

    public abstract IEnumerable<IParser> GetChildren();
}