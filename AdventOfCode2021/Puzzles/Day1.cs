using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2021.Puzzles;

public class Day1 : Puzzle
{
    public Day1()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var count = Input.Ints().Pairwise((a, b) => b > a).Count(true);
        WriteLn(count);
    }

    public override void PartTwo()
    {
        var count = Input.Ints().Window(3).Select(w => w.Sum()).Pairwise((a, b) => b > a).Count(true);
        WriteLn(count);
    }
}