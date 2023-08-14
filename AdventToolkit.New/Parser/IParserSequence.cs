namespace AdventToolkit.New.Parser;

public interface IParserSequence<TValue> : IParser<IParserSequence<TValue>, TValue>
{
    IEnumerable<TValue> Values { get; }
}