using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day19 : Puzzle
{
    public override void PartOne()
    {
        var count = InputLine.AsInt();
        var low = 1;
        var diff = 1;
        while (count > 1)
        {
            diff *= 2;
            if (count % 2 == 1)
            {
                low += diff;
            }
            count /= 2;
        }
        
        WriteLn(low);
    }

    public override void PartTwo()
    {
        var count = InputLine.AsInt();
        
        // If count is a power of 3, last elf wins.
        // Between two consecutive powers of 3, A and B, the first A elves will win consecutively.
        // From A*2 elves to B, the winning number increments by two.
        var pow = 3.Pow(count.Log(3));
        if (pow == count) WriteLn(count);
        else if (count < pow * 2) WriteLn(count - pow);
        else WriteLn(count * 2 - pow * 3);
    }
}