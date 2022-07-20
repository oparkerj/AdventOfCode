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
            // Edge case, interval ends at int max.
            int offset;
            if (interval.Last == int.MaxValue)
            {
                offset = _intervals[affected.Start].Start < (long) interval.Last + 1 ? affected.Start + 1 : affected.Start;
            }
            else
            {
                offset = _intervals[affected.Start].Start < interval.End ? affected.Start + 1 : affected.Start;
            }
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

    public IList<Interval> Intervals => _intervals.AsReadOnly();

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

    public void Remove(Interval interval)
    {
        if (interval.Length == 0) return;
        var check = GetOverlaps(interval);
        if (check.Length == 0) return;

        var first = -1;
        var last = -1;
        if (CheckOutside(interval, _intervals[check.Start]))
        {
            first = check.Start;
            check = new Interval(check.Start + 1, check.Length - 1);
        }
        if (check.Length > 0 && CheckOutside(interval, _intervals[check.Last]))
        {
            last = check.Last;
            check = new Interval(check.Start, check.Length - 1);
        }

        if (first > -1)
        {
            if (CheckSplit(_intervals[first], interval))
            {
                var chunk = _intervals[first];
                _intervals[first] = Interval.Range(chunk.Start, interval.Start);
                _intervals.Insert(first + 1, Interval.Range(interval.End, chunk.End));
                return;
            }

            _intervals[first] = Interval.Range(_intervals[first].Start, interval.Start);
        }
        if (last > -1)
        {
            _intervals[last] = Interval.Range(interval.End, _intervals[last].End);
        }

        _intervals.RemoveRange(check.Start, check.Length);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<int> GetEnumerator() => _intervals.SelectMany(i => i).GetEnumerator();

    public int Sum() => _intervals.Select(interval => interval.Sum()).Sum();

    public int Max() => _intervals[^1].Last;

    public int Min() => _intervals[0].Start;
}