using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;

namespace AdventOfCode2021.Puzzles;

public class Day5 : Puzzle
{
    public Day5()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var grid = new Grid<int>();
        foreach (var rect in Input.Select(s => s.SingleSplit(" -> ").Select(Pos.Parse).ToRect()))
        {
            if (rect.Height > 1 && rect.Width > 1) continue;
            foreach (var pos in rect)
            {
                grid[pos]++;
            }
        }
        WriteLn(grid.Values.Count(i => i > 1));
    }

    public override void PartTwo()
    {
        var grid = new Grid<int>();
        foreach (var (a, b) in Input.Select(s => s.SingleSplit(" -> ").Select(Pos.Parse)))
        {
            foreach (var pos in a.Connect(b))
            {
                grid[pos]++;
            }
        }
        WriteLn(grid.Values.Count(i => i > 1));
    }
}