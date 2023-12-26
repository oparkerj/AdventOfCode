using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2023.Puzzles;

public class Day12 : Puzzle<int, long>
{
    public override int PartOne()
    {
        var rows = Input.Select(s => s.SingleSplit(' ')).Select(tuple => (tuple.Left, tuple.Right.Csv().Ints().ToList()));
        var realTotal = 0;
        foreach (var (given, groups) in rows)
        {
            realTotal += CountPossible(given, groups);
        }
        return realTotal;
        
        int CountPossible(string given, List<int> groups)
        {
            var requiredMask = (uint) given.Select(c => c == '#' ? 1 : 0).BitsToInt();
            var emptyMask = (uint) given.Select(c => c == '.' ? 0 : 1).BitsToInt();
            var total = 0;
            for (var i = 0U; i < (1 << 20); i++)
            {
                if ((i | requiredMask) != i || (i & emptyMask) != i) continue;
                if (i.Bits().RunLengthEncode().WhereKey(true).Values().SequenceEqual(groups))
                {
                    total++;
                }
            }
            return total;
        }
    }

    public override long PartTwo()
    {
        var rows = Input.Select(s => s.SingleSplit(' ')).Select(tuple => (tuple.Left, tuple.Right.Csv().Ints().ToList()));
        
        var realTotal = 0L;
        foreach (var (given, groups) in rows)
        {
            // Repeat 5 times (joined by ?), and have . at the end
            var unfolded = (given + "?").Repeat(5)[..^1] + ".";
            realTotal += CountPossible(unfolded, groups.Repeat(5).ToList());
        }
        return realTotal;

        long CountPossible(string given, List<int> groups)
        {
            // (first N spaces, number of groups, Length of last group)
            var ways = new DefaultDict<(int Index, int Groups, int Length), long>();
            ways[(0, 0, 0)] = 1;

            for (var i = 0; i < given.Length; i++)
            {
                for (var g = 0; g <= groups.Count; g++)
                {
                    for (var len = 0; len <= given.Length; len++)
                    {
                        if (!ways.TryGetValue((i, g, len), out var count)) continue;
                        if (given[i] is '.' or '?' && (len == 0 || len == groups[g - 1])) ways[(i + 1, g, 0)] += count;
                        if (given[i] is '#' or '?') ways[(i + 1, g + (len == 0).AsInt(), len + 1)] += count;
                    }
                }
            }
            return ways[(given.Length, groups.Count, 0)];
        }
    }
}