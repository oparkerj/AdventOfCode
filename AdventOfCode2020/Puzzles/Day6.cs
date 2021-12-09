using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles;

public class Day6 : Puzzle
{
    public Day6()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var sum = Groups.JoinEach().Select(s => s.Distinct().Count()).Sum();
        WriteLn(sum);
    }

    public override void PartTwo()
    {
        var sum = Groups.Select(strings => strings.Aggregate((a, b) => a.Intersect(b).Str()).Length).Sum();
        WriteLn(sum);
    }
}