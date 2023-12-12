using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2023.Puzzles;

public class Day12 : Puzzle<int>
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
}