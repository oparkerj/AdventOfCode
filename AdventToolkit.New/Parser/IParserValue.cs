namespace AdventToolkit.New.Parser;

public interface IParserValue<TValue> : IParser<IParserValue<TValue>, TValue>
{
    TValue Value { get; }
}