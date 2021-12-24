using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections;

public readonly struct Interval : IEnumerable<int>
{
    public readonly int Start;
    public readonly int Length;

    public Interval(int start, int length)
    {
        Start = start;
        Length = length;
    }
        
    public Interval(int length) : this(0, length) { }

    public static implicit operator Interval(Range r)
    {
        if (r.Start.IsFromEnd || r.End.IsFromEnd) throw new Exception("Range indices cannot be from end.");
        return Range(r.Start.Value, r.End.Value);
    }

    public static implicit operator Range(Interval i) => new(i.Start, i.Start + i.Length);

    public static implicit operator Interval(int i) => new(i, 1);

    public static Interval Range(int start, int end) => new(start, end - start);
        
    public static Interval RangeInclusive(int start, int end) => new(start, end - start + 1);

    public static Interval Count(int count, int start = 0) => new(start, count);

    public int End => Start + Length;

    public int Last => End - 1;

    public bool Contains(int i) => i >= Start && i < End;

    // Be careful not to use both this method and RangeInclusive
    public bool ContainsInclusive(int i) => i >= Start && i <= End;

    public Interval Overlap(Interval other)
    {
        var (a, b) = (this, other);
        if (b.Start < a.Start) (a, b) = (b, a);
        if (b.Start < a.End) return new Interval(b.Start, Math.Min(a.End - b.Start, b.Length));
        return new Interval();
    }

    public Interval Shift(int amount) => new(Start + amount, Length);

    // Tell if the number is in the lower half of the range.
    // equal is the value to return if the number is exactly at the midpoint of the range.
    public bool InLowerHalf(int i, bool equal = false)
    {
        i -= Start;
        var half = Length / 2;
        return i == half && Length % 2 == 0 ? equal : i <= half;
    }

    public int Sum() => Algorithms.SumRange(Start, Last);

    public int Min() => Start;

    public int Max() => Last;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<int> GetEnumerator() => Enumerable.Range(Start, Length).GetEnumerator();

    public bool Equals(Interval other)
    {
        return Start == other.Start && Length == other.Length;
    }

    public override bool Equals(object obj)
    {
        return obj is Interval other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, Length);
    }
}