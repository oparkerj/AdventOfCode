using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day18 : Puzzle
{
    public const char Safe = '.';
    public const char Trap = '^';

    public int Rows = 40;
    
    public string NextRow(string current)
    {
        return string.Create(current.Length, current, (span, c) =>
        {
            var left = false;
            var mid = c[0] == Trap;

            for (var i = 0; i < span.Length; i++)
            {
                var right = i != span.Length - 1 && c[i + 1] == Trap;
                span[i] = (left == mid) != (mid == right) ? Trap : Safe;
                (left, mid) = (mid, right);
            }
        });
    }

    public override void PartOne()
    {
        var current = InputLine;
        var total = current.Count(Safe);
        var count = 1;
        while (count < Rows)
        {
            current = NextRow(current);
            total += current.Count(Safe);
            count++;
        }
        
        WriteLn(total);
    }

    public override void PartTwo()
    {
        Rows = 400000;
        PartOne();
    }
}