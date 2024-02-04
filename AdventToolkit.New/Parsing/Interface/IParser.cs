namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// A generic untyped parser.
/// This should only be used as a parameter or return type.
/// Implementing classes should use <see cref="IParser{TIn, TOut}"/>.
/// </summary>
public interface IParser
{
    /// <summary>
    /// Get all parsers which are children of this parser.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IParser> GetChildren()
    {
        yield break;
    }
}

/// <summary>
/// Represents a transformation from one type to another.
/// </summary>
/// <typeparam name="TIn">Input type.</typeparam>
/// <typeparam name="TOut">Output type.</typeparam>
public interface IParser<in TIn, out TOut> : IParser
{
    /// <summary>
    /// Perform the conversion.
    /// </summary>
    /// <param name="input">Input value.</param>
    /// <returns>Converted value.</returns>
    TOut Parse(TIn input);
}