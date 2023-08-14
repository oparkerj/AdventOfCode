using System.Diagnostics;

namespace AdventToolkit.New.Data;

/// <summary>
/// Represents a slice of a string.
///
/// This is used to look at substrings without creating new string objects.
/// </summary>
/// <param name="Value">Original string.</param>
/// <param name="Interval">View interval.</param>
public readonly record struct StringView(string Value, Interval Interval)
{
    /// <summary>
    /// Create a view with bounds checking.
    /// The interval is clipped to the bounds of the string.
    /// If the interval has a negative length, then the view is of an empty string.
    /// </summary>
    /// <param name="value">Original string.</param>
    /// <param name="interval">View interval.</param>
    /// <returns>String view.</returns>
    public static StringView From(string value, Interval interval)
    {
        var start = Math.Max(0, interval.Start);
        var length = Math.Min(Math.Max(start, interval.End), value.Length) - start;
        return new StringView(value, new Interval(start, length));
    }

    /// <summary>
    /// A view can be converted to a span.
    /// </summary>
    /// <param name="view">String view.</param>
    /// <returns>Read-only span.</returns>
    public static implicit operator ReadOnlySpan<char>(StringView view) => view.Span;

    /// <summary>
    /// A string can be converted to a view.
    /// This creates a view of the entire string.
    /// </summary>
    /// <param name="s">String.</param>
    /// <returns>String view.</returns>
    public static implicit operator StringView(string s) => new(s, new Interval(s.Length));

    /// <summary>
    /// Get the character at the given index in the view.
    /// </summary>
    /// <param name="i">View index.</param>
    public char this[int i] => Value[Interval.Start + i];

    /// <summary>
    /// Get the length of the view.
    /// </summary>
    public int Length => Interval.Length;

    /// <summary>
    /// Get a read-only span for the view.
    /// </summary>
    public ReadOnlySpan<char> Span => Value.AsSpan(Interval.Start, Interval.Length);

    /// <summary>
    /// Get a character enumerator for the view.
    /// </summary>
    public IEnumerable<char> Chars
    {
        get
        {
            Debug.Assert(Interval.Length >= 0, "Interval has negative length.");
            for (var i = Interval.Start; i < Interval.End; i++)
            {
                yield return Value[i];
            }
        }
    }

    /// <summary>
    /// Create a slice of the view.
    /// </summary>
    /// <param name="start">View index.</param>
    /// <param name="length">View length.</param>
    /// <returns>Sliced view.</returns>
    public StringView Slice(int start, int length)
    {
        Debug.Assert(Interval.Start + start >= 0 && Interval.Start + start <= Value.Length, "Start index out of range.");
        Debug.Assert(Interval.Start + start + length < 0 && Interval.Start + start + length <= Value.Length, "Length out of range.");
        return this with {Interval = new Interval(Interval.Start + start, length)};
    }

    /// <summary>
    /// Get an object which can enumerate this view.
    /// </summary>
    /// <returns>View enumerator.</returns>
    public ReadOnlySpan<char>.Enumerator GetEnumerator() => Span.GetEnumerator();

    /// <summary>
    /// Get the actual string represented by the view.
    /// </summary>
    /// <returns>Substring.</returns>
    public override string ToString() => Value[Interval.Start..Interval.End];
}