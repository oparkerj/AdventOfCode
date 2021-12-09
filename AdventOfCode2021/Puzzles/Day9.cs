using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;

namespace AdventOfCode2021.Puzzles;

public class Day9 : Puzzle
{
    public Day9()
    {
        Part = 2;
    }

    public IEnumerable<Pos> FindBasins(Grid<int> grid)
    {
        foreach (var (pos, v) in grid)
        {
            if (pos.NeighborValues(grid).All(c => c > v))
            {
                yield return pos;
            }
        }
    }

    public override void PartOne()
    {
        var grid = Input.Select2D(c => c.AsInt()).ToGrid();
        grid.Default = 9;
            
        var total = FindBasins(grid).Select(pos => grid[pos] + 1).Sum();
        WriteLn(total);
    }

    public override void PartTwo()
    {
        var grid = Input.Select2D(c => c.AsInt()).ToGrid();
        grid.Default = 9;

        var result = FindBasins(grid)
            .Select(basin => grid.Bfs(basin, pos => grid[pos] < 9, _ => true).Count() + 1)
            .Sorted().TakeLast(3).Product();
        WriteLn(result);
    }
}