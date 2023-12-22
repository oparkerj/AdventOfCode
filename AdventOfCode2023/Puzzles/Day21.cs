using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day21 : Puzzle<long>
{
    public Day21()
    {
        Part = 1;
    }

    public override long PartOne()
    {
        var grid = Input.ToGrid();
        
        var reachable = grid.DijkstraFrom(grid.Find('S'), pos => grid[pos] == '.');

        if (Part == 1)
        {
            return reachable.Values.Count(tuple => tuple.Dist.Even() && tuple.Dist <= 64);
        }
        else
        {
            // Number of steps is designed so you will exactly walk to the edge of one of
            // the infinite tiles.
            const long steps = 26501365;
            var half = grid.Bounds.Width / 2;
            var tiles = (steps - half) / grid.Bounds.Width;

            // The center row and column is empty, so the resulting area of travel will be a diamond.
            // Here calculate the number of whole grids that are contained in that area, as well as
            // the are of the partial grids that the edge passes through.
            var oddGrids = reachable.Values.Count(tuple => tuple.Dist.Odd());
            var evenGrids = reachable.Values.Count(tuple => tuple.Dist.Even());
            var oddCorners = reachable.Values.Count(tuple => tuple.Dist.Odd() && tuple.Dist > half);
            var evenCorners = reachable.Values.Count(tuple => tuple.Dist.Even() && tuple.Dist > half);
        
            return (tiles + 1) * (tiles + 1) * oddGrids
                   + tiles * tiles * evenGrids
                   - (tiles + 1) * oddCorners
                   + tiles * evenCorners;
        }
    }
}