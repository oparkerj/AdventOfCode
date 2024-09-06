using AdventToolkit2.Parsing.Core;

namespace AdventToolkit2.Parsing.Builtin;

/// <summary>
/// Wrapper for a value that can be adapted in the parsing library.
/// </summary>
/// <param name="Value"></param>
/// <typeparam name="T"></typeparam>
public readonly record struct AdaptValue<T>(T Value)
{
    /// <summary>
    /// Adapt the value with no format specifier.
    /// This will directly adapt from the input type to the output type.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public TOut Into<TOut>() => Into<TOut>($"");
    
    /// <summary>
    /// Adapt the value using the provided parse format.
    /// </summary>
    /// <param name="adapt">Format.</param>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public TOut Into<TOut>(Adapt<T, TOut> adapt) => adapt.Parse(Value);
}