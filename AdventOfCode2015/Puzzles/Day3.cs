using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day3 : Puzzle
{
    public Grid<int> Houses = new();

    public override void PartOne()
    {
        var points = InputLine.Select(Pos.RelativeDirection).Scan(Pos.Origin, Pos.Add);
        foreach (var point in points)
        {
            Houses[point]++;
        }
        WriteLn(Houses.Count);
    }

    public override void PartTwo()
    {
        var offsets = InputLine.Select(Pos.RelativeDirection).ToList();
        var santa = offsets.TakeEvery(2).Scan(Pos.Origin, Pos.Add);
        var roboSanta = offsets.Skip(1).TakeEvery(2).Scan(Pos.Origin, Pos.Add);
        foreach (var point in santa.Concat(roboSanta))
        {
            Houses[point]++;
        }
        WriteLn(Houses.Count);
    }
}