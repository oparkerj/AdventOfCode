using System.Collections.Generic;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using MoreLinq;

namespace AdventOfCode2021.Puzzles;

public class Day11 : Puzzle
{
    public Day11()
    {
        Part = 2;
    }

    public int Step(FixedGrid<int> grid)
    {
        var flashes = 0;
        var inc = new Queue<Pos>(grid.Bounds);
        while (inc.Count > 0)
        {
            var next = inc.Dequeue();
            if (++grid[next] == 10)
            {
                flashes++;
                grid.GetNeighbors(next).ForEach(inc.Enqueue);
            }
        }
        grid.WhereValue(i => i > 9).Keys().ForEach(pos => grid[pos] = 0);
        return flashes;
    }

    public override void PartOne()
    {
        // Fixedgrid is used because writes to areas outside
        // the grid are discarded
        var grid = Input.Select2D(c => c.AsInt()).ToFixedGrid(10, 10);
        grid.IncludeCorners = true;

        var total = 0;
        for (var i = 0; i < 100; i++)
        {
            total += Step(grid);
        }
        WriteLn(total);
    }

    public override void PartTwo()
    {
        var grid = Input.Select2D(c => c.AsInt()).ToFixedGrid(10, 10);
        grid.IncludeCorners = true;

        var step = 0;
        while (true)
        {
            step++;
            if (Step(grid) == 100) break;
        }
        WriteLn(step);
    }
}