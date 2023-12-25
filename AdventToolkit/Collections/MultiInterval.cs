using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Collections;

// Represents multiple discontinuous intervals.
// Added intervals are automatically combined with overlapping and adjacent intervals.
// Known edge cases not accounted for: Remove will not work properly if the interval ends at int max.
public class MultiInterval : IEnumerable<int>
{
    private List<Interval> _intervals;
    private IComparer<Interval> _comparer = Comparer<Interval>.Create((a, b) => a.Start.CompareTo(b.Start));

    public MultiInterval()
    {
        _intervals = new List<Interval>();
    }

    private int Find(int i)
    {
        return _intervals.BinarySearch(i, _comparer);
    }

    private Interval Combine(Interval a, Interval b)
    {
        // Inclusive on Last instead of exclusive on End to account for int max.
        return Interval.RangeInclusive(Math.Min(a.Start, b.Start), Math.Max(a.Last, b.Last));
    }

    // Get the indices of intervals that are overlapped by this one.
    // If the returned interval has length 0, the start point is where to insert this one.
    private Interval GetOverlaps(Interval interval)
    {
        int Lookup(int point)
        {
            var index = Find(point);
            if (index > -1) return index;
            index = ~index - 1;
            return index < 0 ? 0 : index;
        }

        if (_intervals.Count == 0) return new Interval();
        
        var affected = Interval.RangeInclusive(Lookup(interval.Start), Lookup(interval.Last));
        if (!_intervals[affected.Start].Overlaps(interval))
        {
            // Test against Last instead of End to account for int max.
            var offset = _intervals[affected.Start].Start <= interval.Last ? affected.Start + 1 : affected.Start;
            affected = new Interval(offset, affected.Length - 1);
        }
        return affected;
    }

    private bool CheckTouch(Interval a, Interval b)
    {
        return a.End == b.Start;
    }

    private bool CheckOutside(Interval i, Interval check)
    {
        return check.Start < i.Start || check.End > i.End;
    }

    private bool CheckSplit(Interval i, Interval check)
    {
        return i.Start < check.Start && check.End < i.End;
    }

    public int IntervalCount => _intervals.Count;

    public int Count => _intervals.Select(i => i.Length).Sum();

    public IList<Interval> Intervals => _intervals.AsReadOnly();

    public void Clear() => _intervals.Clear();

    public bool Contains(int i)
    {
        if (_intervals.Count == 0) return false;
        var index = Find(i);
        if (index > -1) return true;
        index = ~index - 1;
        return index >= 0 && _intervals[index].Contains(i);
    }

    public void Add(Interval interval)
    {
        if (interval.Length == 0) return;
        var check = GetOverlaps(interval);
        
        if (check.Start > 0 && CheckTouch(_intervals[check.Start - 1], interval))
        {
            check = new Interval(check.Start - 1, check.Length + 1);
        }
        if (check.End < _intervals.Count && CheckTouch(interval, _intervals[check.End]))
        {
            check = new Interval(check.Start, check.Length + 1);
        }
        // Interval does not overlap or touch other intervals, so just insert
        if (check.Length == 0)
        {
            _intervals.Insert(check.Start, interval);
            return;
        }
        
        var replace = Combine(_intervals[check.Start], check.Length > 1 ? Combine(interval, _intervals[check.Last]) : interval);
        _intervals.RemoveRange(check.Start + 1, check.Length - 1);
        _intervals[check.Start] = replace;
    }

    public void AddRange(IEnumerable<Interval> intervals)
    {
        foreach (var interval in intervals)
        {
            Add(interval);
        }
    }

    // Copied from new toolkit version
    public void Remove(Interval interval)
    {
        if (interval.Length == 0) return;

        var overlaps = GetOverlaps(interval);
        if (overlaps.Length == 0) return;

        var first = _intervals[overlaps.Start];

        // If the interval is contained entirely within the first interval
        // then split it into two.
        if (first.Contains(interval))
        {
            if (first.Start == interval.Start)
            {
                _intervals[overlaps.Start] = Interval.Range(interval.End, first.End);
                return;
            }
            if (first.End != interval.End)
            {
                _intervals.Insert(overlaps.Start + 1, Interval.Range(interval.End, first.End));
            }
            _intervals[overlaps.Start] = new Interval(first.Start, interval.Start - first.Start);
            return;
        }

        // Adjust first interval if partially outside
        if (first.Start < interval.Start)
        {
            _intervals[overlaps.Start] = new Interval(first.Start, interval.Start - first.Start);
            overlaps = new Interval(overlaps.Start + 1, overlaps.Length - 1);
        }

        // Adjust last interval if partially outside
        var last = _intervals[overlaps.Last];
        if (interval.Last < last.Last)
        {
            _intervals[overlaps.Last] = Interval.Range(interval.End, last.End);
            overlaps = new Interval(overlaps.Start, overlaps.Length - 1);
        }

        // Delete everything in the middle
        _intervals.RemoveRange(overlaps.Start, overlaps.Length);
    }

    // Copied from new toolkit version
    public IEnumerable<Interval> Intersect(Interval interval)
    {
        var overlaps = GetOverlaps(interval);
        if (overlaps.Length == 0) yield break;

        yield return interval.Overlap(_intervals[overlaps.Start]);
        if (overlaps.Length <= 1) yield break;

        var i = overlaps.Start + 1;
        for (; i < overlaps.Last; ++i)
        {
            yield return _intervals[i];
        }
        yield return interval.Overlap(_intervals[i]);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<int> GetEnumerator() => _intervals.SelectMany(i => i).GetEnumerator();

    public int Sum() => _intervals.Select(interval => interval.Sum()).Sum();

    public int Max() => _intervals[^1].Last;

    public int Min() => _intervals[0].Start;

    public override string ToString()
    {
        return string.Join(", ", _intervals);
    }
}