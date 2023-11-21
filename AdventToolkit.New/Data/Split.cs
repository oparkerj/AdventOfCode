namespace AdventToolkit.New.Data;

/// <summary>
/// Represents a span that has been split into two parts.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly ref struct Split<T>
{
    public readonly Span<T> First;
    public readonly Span<T> Second;

    public Split(Span<T> first, Span<T> second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// Create a split using the given split index.
    /// </summary>
    /// <param name="span">Input span.</param>
    /// <param name="split">Split index, this index will be the first element
    /// of the second split.</param>
    /// <returns></returns>
    public static Split<T> From(Span<T> span, int split)
    {
        return new Split<T>(span[..split], span[split..]);
    }

    /// <summary>
    /// Create a reversed split using the given split index.
    /// This is the same as <see cref="From"/> but the first and second
    /// spans are swapped.
    /// </summary>
    /// <param name="span">Input span.</param>
    /// <param name="split">Split index, this index will be the first element
    /// of the first split.</param>
    /// <returns></returns>
    public static Split<T> FromReverse(Span<T> span, int split)
    {
        return new Split<T>(span[split..], span[..split]);
    }

    public void Deconstruct(out Span<T> first, out Span<T> second)
    {
        first = First;
        second = Second;
    }
}