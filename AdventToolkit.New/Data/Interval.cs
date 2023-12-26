using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventToolkit.New.Extensions;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

// TODO: .NET 8, make generic and create alias
/// <summary>
/// Represents a range of integers.
/// </summary>
/// <param name="Start">Start of the interval.</param>
/// <param name="Length">Length of the interval.</param>
[CollectionBuilder(typeof(IntervalExtensions), nameof(IntervalExtensions.Create))]
public readonly record struct Interval<T>(T Start, T Length) : IBound<Interval<T>, T>
    where T : INumber<T>
{
    /// <summary>
    /// Create an interval starting from 0 with the given length.
    /// </summary>
    /// <param name="length">Interval length.</param>
    public Interval(T length) : this(T.Zero, length) { }
    
    public static Interval<T> Empty => new(T.Zero, T.Zero);

    /// <summary>
    /// Create an interval from the start and end points.
    /// </summary>
    /// <param name="start">Start value.</param>
    /// <param name="end">End value (exclusive).</param>
    /// <returns>Interval from [start, end).</returns>
    public static Interval<T> From(T start, T end)
    {
        Debug.Assert(start <= end, "Start is after end.");
        return new Interval<T>(start, end - start);
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
    public static Interval<T> Span(T a, T b) => new(T.Min(a, b), T.Abs(a - b) + T.One);

    /// <summary>
    /// Convert a number to an interval containing only that number.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static implicit operator Interval<T>(T t) => new(t, T.One);

    /// <summary>
    /// Cast the interval to another number type.
    /// The values are cast in a truncating manner.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    /// <returns></returns>
    public Interval<TOut> As<TOut>()
        where TOut : INumber<TOut> => new(TOut.CreateTruncating(Start), TOut.CreateTruncating(Length));

    public T Min => Start;

    public T Max => Last;

    public T Size => Length;

    /// <summary>
    /// Get the end of the interval.
    /// This is the value after the last value in the interval.
    /// </summary>
    public T End => Start + Length;

    /// <summary>
    /// Get the last value contained in the interval.
    /// </summary>
    public T Last => End - T.One;

    /// <summary>
    /// Check if a value is in the interval.
    /// </summary>
    /// <param name="i">Value.</param>
    /// <returns>True if the value is in the interval, false otherwise.</returns>
    public bool Contains(T i) => i >= Start && i <= Last;
    
    public bool Contains(Interval<T> t)
    {
        return t.Start >= Start && t.Last <= Last;
    }

    /// <summary>
    /// Get the intersection of this interval and another.
    ///
    /// If the intervals do not overlap, the result will have Length = 0.
    /// </summary>
    /// <param name="other">Other interval.</param>
    /// <returns>Interval intersection.</returns>
    public Interval<T> Intersect(Interval<T> other)
    {
        if (other.Start < Start)
        {
            if (Start <= other.Last)
            {
                return this with {Length = T.Min(other.End - Start, Length)};
            }
            return default;
        }
        if (other.Start <= Last)
        {
            return other with {Length = T.Min(End - other.Start, other.Length)};
        }
        return default;
    }

    /// <summary>
    /// Check if this interval intersects another.
    /// </summary>
    /// <param name="other">Other interval.</param>
    /// <returns>True if the intervals overlap, false otherwise.</returns>
    public bool Intersects(Interval<T> other)
    {
        return other.Start < Start ? Start < other.End : other.Start < End;
    }

    /// <summary>
    /// Expand the interval to fit the given value.
    /// </summary>
    /// <param name="i">Value.</param>
    /// <returns>Expanded interval.</returns>
    public Interval<T> Fit(T i)
    {
        if (Length == T.Zero) return new Interval<T>(i, T.One);
        var start = T.Min(Start, i);
        return new Interval<T>(start, T.Max(Length, i - start + T.One));
    }

    /// <summary>
    /// Shift the interval while maintaining the length.
    /// </summary>
    /// <param name="startOffset">Shift amount.</param>
    /// <returns>Shifted interval.</returns>
    public Interval<T> Offset(T startOffset) => this with {Start = Start + startOffset};

    /// <summary>
    /// Shift the start and end of the interval.
    /// </summary>
    /// <param name="startOffset"></param>
    /// <param name="endOffset"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public Interval<T> Offset(T startOffset, T endOffset)
    {
        var start = Start + startOffset;
        Debug.Assert(start <= End + endOffset, $"{nameof(Interval<T>)} has negative length.");
        return new Interval<T>(start, End + endOffset - start);
    }

    /// <summary>
    /// Expand the interval to the left.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Interval<T> ExpandLeft(T amount)
    {
        Debug.Assert(Length + amount >= T.Zero, "Length is negative.");
        return new Interval<T>(Start - amount, Length + amount);
    }

    /// <summary>
    /// Expand the interval to the right.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Interval<T> ExpandRight(T amount)
    {
        Debug.Assert(Length + amount >= T.Zero, "Length is negative.");
        return this with {Length = Length + amount};
    }

    /// <summary>
    /// Slice the interval.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public Interval<T> Slice(T start, T length) => new(Start + start, length);

    /// <summary>
    /// Slice the interval using a range.
    /// </summary>
    /// <param name="r"></param>
    public Interval<T> this[Range r]
    {
        get
        {
            var (start, end) = r.GetOffsets(Length);
            return From(start, end);
        }
    }

    /// <summary>
    /// Get the value in the interval at the given index.
    /// </summary>
    /// <param name="i"></param>
    public T this[Index i] => Start + i.GetOffset(Length);

    /// <summary>
    /// Get the value in the interval at the given offset.
    /// </summary>
    /// <param name="offset"></param>
    public T this[T offset] => Start + offset;

    /// <summary>
    /// Copy the interval sequence to the span.
    /// </summary>
    /// <param name="span"></param>
    public void CopyTo(Span<T> span)
    {
        if (Length == T.Zero) return;
        Debug.Assert(Length > T.Zero, "Invalid length");
        Debug.Assert(span.Length >= int.CreateChecked(Length), "");

        var i = 0;
        var t = Start;
        for (; t <= Last; ++i, ++t)
        {
            span[i] = t;
        }
    }

    public override string ToString() => Length > T.Zero ? $"[{Start}, {Last}]" : $"[{Start}]";

    public Enumerator GetEnumerator() => new(Start, End);
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerate each value in the interval.
    /// </summary>
    public struct Enumerator : IEnumerator<T>
    {
        private readonly T _end;

        public T Current { get; private set; }

        public Enumerator(T current, T end)
        {
            Current = current - T.One;
            _end = end;
        }

        public bool MoveNext() => ++Current < _end;

        object IEnumerator.Current => Current;

        public void Dispose() { }

        public void Reset() => throw new NotSupportedException();
    }
}