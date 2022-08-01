using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day20 : Puzzle
{
    public int Target;

    public Day20() => Target = InputInt;

    public bool CheckValid(int i)
    {
        var total = 0;
        for (var d = 1; d * d <= i; d++)
        {
            if (i % d != 0) continue;
            total += d * 10;
            if (i / d is var factor && factor != d) total += factor * 10;
        }
        return total >= Target;
    }

    public override void PartOne()
    {
        var result = Num.Positive().First(CheckValid);
        WriteLn(result);
    }
    
    public bool CheckValid2(int i)
    {
        var total = 0;
        for (var d = 1; d * d <= i; d++)
        {
            if (i % d != 0) continue;
            if (i <= d * 50) total += d * 11;
            if (i / d is var factor && factor != d && i <= factor * 50) total += factor * 11;
        }
        return total >= Target;
    }

    public override void PartTwo()
    {
        var result = Num.Positive().First(CheckValid2);
        WriteLn(result);
    }
}