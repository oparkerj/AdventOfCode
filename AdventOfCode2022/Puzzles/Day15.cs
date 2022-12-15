using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day15 : Puzzle<int, long>
{
    public override int PartOne()
    {
        var info = new Dictionary<Pos, Pos>();

        foreach (var s in Input)
        {
            var p = s.GetInts().ToArray(4);
            info[new Pos(p[0], p[1])] = new Pos(p[2], p[3]);
        }

        const int target = 2000000;

        var ranges = new MultiInterval();
        foreach (var sensor in info.Keys)
        {
            var beacon = info[sensor];
            var dist = sensor.MDist(beacon);
            if (Math.Abs(sensor.Y - target) <= dist)
            {
                var width = (dist - Math.Abs(sensor.Y - target) + 1) * 2 - 1;
                ranges.Add(Interval.RangeInclusive(sensor.X - width / 2, sensor.X + width / 2));
            }
        }
        
        var count = ranges.Count;
        foreach (var pos in info.Keys.Concat(info.Values).Distinct())
        {
            if (ranges.Contains(pos.X) && pos.Y == target) count--;
        }
        return count;
    }

    public override long PartTwo()
    {
        var info = new Dictionary<Pos, Pos>();

        foreach (var s in Input)
        {
            var array = s.GetInts().ToArray();
            info[new Pos(array[0], array[1])] = new Pos(array[2], array[3]);
        }

        var possible = info.Select(pair => pair.Key.GetMDistRing(pair.Key.MDist(pair.Value) + 1))
            .Flatten()
            .WhereMultiple();

        foreach (var pos in possible)
        {
            if (pos.X is < 0 or > 4000000 || pos.Y is < 0 or > 4000000) continue;
            if (info.All(pair => pos.MDist(pair.Key) > pair.Key.MDist(pair.Value)))
            {
                return (long) pos.X * 4000000 + pos.Y;
            }
        }
        
        return -1;
    }
}