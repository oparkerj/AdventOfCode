using System.Diagnostics;

namespace AdventToolkit.New.Data;

// TODO: .NET 8, make generic and create alias
/// <summary>
/// Represents a range of integers.
/// </summary>
/// <param name="Start">Start of the interval.</param>
/// <param name="Length">Length of the interval.</param>
public readonly record struct Interval(int Start, int Length)
{
    /// <summary>
    /// Create an interval starting from 0 with the given length.
    /// </summary>
    /// <param name="length">Interval length.</param>
    public Interval(int length) : this(0, length) { }

    /// <summary>
    /// Create an interval from the start and end points.
    /// </summary>
    /// <param name="start">Start value.</param>
    /// <param name="end">End value (exclusive).</param>
    /// <returns>Interval from [start, end).</returns>
    public static Interval From(int start, int end)
    {
        Debug.Assert(start <= end, "Start is after end.");
        return new Interval(start, end - start);
    }

    /// <summary>
    /// Get an integer that inclusively spans between two values.
    ///
    /// This method will produce an interval with a positive length
    /// which is always at least 1.
    /// </summary>
    /// <param name="a">First endpoint.</param>
    /// <param name="b">Second endpoint.</param>
    /// <returns>Interval form [min(a, b), max(a, b)].</returns>
    public static Interval Span(int a, int b) => new(Math.Min(a, b), Math.Abs(a - b) + 1);

    /// <summary>
    /// Get the end of the interval.
    /// This is the value after the last value in the interval.
    /// </summary>
    public int End => Start + Length;

    /// <summary>
    /// Get the last value contained in the interval.
    /// </summary>
    public int Last => End - 1;

    /// <summary>
    /// Check if a value is in the interval.
    /// </summary>
    /// <param name="i">Value.</param>
    /// <returns>True if the value is in the interval, false otherwise.</returns>
    public bool Contains(int i) => i >= Start && i <= Last;

    /// <summary>
    /// Get the intersection of this interval and another.
    ///
    /// If the intervals do not overlap, the result will have Length = 0.
    /// </summary>
    /// <param name="other">Other interval.</param>
    /// <returns>Interval intersection.</returns>
    public Interval Intersect(Interval other)
    {
        if (other.Start < Start)
        {
            if (Start < other.End)
            {
                return this with {Length = Math.Min(other.End - Start, Length)};
            }
            return default;
        }
        if (other.Start < End)
        {
            return other with {Length = Math.Min(End - other.Start, other.Length)};
        }
        return default;
    }

    /// <summary>
    /// Check if this interval intersects another.
    /// </summary>
    /// <param name="other">Other interval.</param>
    /// <returns>True if the intervals overlap, false otherwise.</returns>
    public bool Intersects(Interval other)
    {
        return other.Start < Start ? Start < other.End : other.Start < End;
    }

    /// <summary>
    /// Expand the interval to fit the given value.
    /// </summary>
    /// <param name="i">Value.</param>
    /// <returns>Expanded interval.</returns>
    public Interval Fit(int i)
    {
        if (Length == 0) return new Interval(i, 1);
        return new Interval(Math.Min(Start, i), Math.Max(Length, i - Start + 1));
    }

    /// <summary>
    /// Shift the interval while maintaining the length.
    /// </summary>
    /// <param name="startOffset">Shift amount.</param>
    /// <returns>Shifted interval.</returns>
    public Interval Offset(int startOffset) => this with {Start = Start + startOffset};

    /// <summary>
    /// Shift the start and end of the interval.
    /// </summary>
    /// <param name="startOffset"></param>
    /// <param name="endOffset"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Interval Offset(int startOffset, int endOffset)
    {
        var start = Start + startOffset;
        Debug.Assert(start <= End + endOffset, $"{nameof(Interval)} has negative length.");
        return new Interval(start, End + endOffset - start);
    }

    public override string ToString() => $"[{Start}, {End})";
}