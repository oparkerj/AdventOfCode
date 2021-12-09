using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2018.Puzzles;

public class Day1 : Puzzle
{
    public Day1()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        WriteLn(Input.Ints().Sum());
    }

    public override void PartTwo()
    {
        WriteLn(Input.Ints().Endless().PreScan(Num.Add, 0).FirstRepeat());
    }
}