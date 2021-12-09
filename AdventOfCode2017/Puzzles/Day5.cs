using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles;

public class Day5 : Puzzle
{
    public Day5()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var jumps = Input.Ints().ToArray();
        var ptr = 0;
        var count = 0;
        while (ptr >= 0 && ptr < jumps.Length)
        {
            ptr += jumps[ptr]++;
            count++;
        }
        WriteLn(count);
    }

    public override void PartTwo()
    {
        var jumps = Input.Ints().ToArray();
        var ptr = 0;
        var count = 0;
        while (ptr >= 0 && ptr < jumps.Length)
        {
            ptr += jumps[ptr] >= 3 ? jumps[ptr]-- : jumps[ptr]++;
            count++;
        }
        WriteLn(count);
    }
}