using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day1 : Puzzle
{
    public override void PartOne()
    {
        var sum = InputLine.QuickMap('(', 1, -1).Sum();
        WriteLn(sum);
    }

    public override void PartTwo()
    {
        var result = InputLine.QuickMap('(', 1, -1)
            .Scan(0, Num.Add)
            .FirstIndex(i => i < 0);
        WriteLn(result);
    }
}