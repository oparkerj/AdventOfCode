using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2021.Puzzles;

public class Day7 : Puzzle
{
    public Day7()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var pos = InputLine.Csv().Ints().ToList();
        var min = pos.Min();
        var max = pos.Max() + 1;

        Interval range = min..max;
        var target = range.Min(i => pos.Select(p => Math.Abs(p - i)).Sum());
        WriteLn(target);
    }

    public override void PartTwo()
    {
        var pos = InputLine.Csv().Ints().ToList();
        var min = pos.Min();
        var max = pos.Max() + 1;

        Interval range = min..max;
        var target = range.Min(i => pos.Select(p => Math.Abs(p - i)).Select(Algorithms.Sum1ToN).Sum());
        WriteLn(target);
    }
}