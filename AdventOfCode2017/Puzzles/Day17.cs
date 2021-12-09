using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles;

public class Day17 : Puzzle
{
    public Day17()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var gap = InputLine.AsInt();
        var buf = new LinkedList<int>();
        var current = buf.AddLast(0);
        foreach (var i in Enumerable.Range(1, 2017))
        {
            current = current.Repeat(ListExtensions.NextCircular, gap);
            current = buf.AddAfter(current, i);
        }
        WriteLn(current.NextCircular().Value);
    }

    public override void PartTwo()
    {
        var gap = InputLine.AsInt();
        var index = 1;
        var neighbor = 1;
        var size = 2;
        foreach (var i in Enumerable.Range(2, 50_000_000 - 1))
        {
            index = (index + gap) % size + 1;
            if (index == 1) neighbor = i;
            size++;
        }
        WriteLn(neighbor);
    }
}