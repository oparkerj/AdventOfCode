namespace AdventToolkit.New.Parser;

public interface IParser<TValue> { }

public interface IParser<TParser, TValue> : IParser<TValue>
    where TParser : IParser<TParser, TValue>
{
    //
}