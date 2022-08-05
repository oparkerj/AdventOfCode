using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return InputLine.QuickMap('(', 1, -1).Sum();
    }

    public override int PartTwo()
    {
        return InputLine.QuickMap('(', 1, -1)
            .Scan(0, Num.Add)
            .FirstIndex(i => i < 0);
    }
}