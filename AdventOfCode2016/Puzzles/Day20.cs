using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day20 : Puzzle
{
    public int Map(uint i)
    {
        return (int) (i + int.MinValue);
    }

    public long Unmap(int i)
    {
        return (long) i - int.MinValue;
    }

    public MultiInterval BuildList()
    {
        var ranges = Input.Extract<(uint, uint)>(@"(\d+)-(\d+)")
            .TupleSelect(Map)
            .SpreadSelect(Interval.RangeInclusive)
            .ToList();

        var intervals = new MultiInterval();
        foreach (var interval in ranges)
        {
            intervals.Add(interval);
        }
        return intervals;
    }
    
    public override void PartOne()
    {
        var list = BuildList();
        var first = list.Intervals[0];
        var min = int.MinValue < first.Start ? int.MinValue : first.End;
        WriteLn(Unmap(min));
    }

    public override void PartTwo()
    {
        var list = BuildList();
        var blocked = list.Intervals.Select(i => i.Length).Longs().Sum();
        WriteLn((1L << 32) - blocked);
    }
}