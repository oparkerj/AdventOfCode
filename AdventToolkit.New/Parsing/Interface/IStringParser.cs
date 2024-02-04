namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// A string parser which provides span parsing.
/// </summary>
/// <typeparam name="T">Output type.</typeparam>
public interface IStringParser<out T> : IParser<string, T>
{
    /// <summary>
    /// Perform the conversion on a span.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    T Parse(ReadOnlySpan<char> span);

    T IParser<string, T>.Parse(string input) => Parse(input.AsSpan());
}