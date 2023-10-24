using System.Numerics;

namespace AdventToolkit.New.Data;

public class MultiInterval<T>
    where T : INumber<T>
{
    private static readonly IComparer<Interval<T>> Comparison = Comparer<Interval<T>>.Create(static (a, b) => a.Start.CompareTo(b.Start));
    
    private readonly List<Interval<T>> _intervals = new();

    /// <summary>
    /// Do a binary search for the given value. The search only considers the
    /// Start value of the intervals.
    ///
    /// If the result is positive, it is the index of a corresponding interval.
    /// This means the value was exactly equal to the start value of an interval.
    ///
    /// If the result is negative, then the bitwise complement gives the index of
    /// where a new start value would be inserted.
    /// In other words, ~result - 1 gives the index of the interval that could
    /// possibly contain the value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int FindInsertIndex(T value)
    {
        return _intervals.BinarySearch(new Interval<T>(value, T.One), Comparison);
    }

    /// <summary>
    /// Get the range of intervals that overlap with the given interval.
    /// If the result has Length = 0 then the start index is where the interval
    /// would be inserted.
    /// </summary>
    /// <param name="interval"></param>
    /// <returns>Overlap indices.</returns>
    private Interval<int> GetOverlaps(Interval<T> interval)
    {
        if (_intervals.Count == 0) return new Interval<int>(0);

        var possible = Interval<int>.Span(PossibleOverlap(interval.Start), PossibleOverlap(interval.Last));
        if (!_intervals[possible.Start].Intersects(interval))
        {
            possible = new Interval<int>(possible.Start + 1, possible.Length - 1);
        }
        return possible;

        // Get the index of the interval that could potentially contain
        // the given value
        int PossibleOverlap(T value)
        {
            var index = FindInsertIndex(value);
            return index > -1 ? index : Math.Max(0, ~index - 1);
        }
    }

    /// <summary>
    /// Merge two intervals that are known to be touching/overlapping.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private Interval<T> Merge(Interval<T> a, Interval<T> b)
    {
        return Interval<T>.Span(T.Min(a.Start, b.Start), T.Max(a.Last, b.Last));
    }

    /// <summary>
    /// Check if a value is in the collection of intervals.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value)
    {
        if (_intervals.Count == 0) return false;

        var possible = FindInsertIndex(value);
        // If the value equals the start of an interval
        if (possible > -1) return true;

        possible = ~possible - 1;
        return possible > -1 && _intervals[possible].Contains(value);
    }

    /// <summary>
    /// Add an interval to the collection.
    /// The collection will be updated such that overlapping and adjacent intervals
    /// will be merged together.
    /// </summary>
    /// <param name="interval"></param>
    public void Add(Interval<T> interval)
    {
        if (interval.Length == T.Zero) return;

        var merge = GetOverlaps(interval);
        if (merge.Start > 0 && IsTouching(_intervals[merge.Start - 1], interval))
        {
            // If touching interval to the left, add to merge
            merge = new Interval<int>(merge.Start - 1, merge.Length + 1);
        }
        if (merge.End < _intervals.Count && IsTouching(interval, _intervals[merge.End]))
        {
            // If touching interval to the right, add to merge
            merge = merge with {Length = merge.Length + 1};
        }

        if (merge.Length == 0)
        {
            // No overlaps, just insert
            _intervals.Insert(merge.Start, interval);
            return;
        }

        // If interval is completely inside first overlap, then no merge
        if (interval.Last <= _intervals[merge.Start].Last) return;

        // Merge with first/last overlaps
        var full = Merge(_intervals[merge.Start], merge.Length > 1 ? Merge(interval, _intervals[merge.Last]) : interval);
        // Remove entries that are no longer needed
        _intervals.RemoveRange(merge.Start + 1, merge.Length - 1);
        // Update with new interval
        _intervals[merge.Start] = full;
        return;

        static bool IsTouching(Interval<T> left, Interval<T> right)
        {
            return left.End == right.Start;
        }
    }

    public void Remove(Interval<T> interval)
    {
        if (interval.Length == T.Zero) return;

        var overlaps = GetOverlaps(interval);
        if (overlaps.Length == 0) return;
        
        // TODO
    }
}