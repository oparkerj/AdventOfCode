using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day18 : Puzzle<long>
{
    public Day18()
    {
        // InputName = "Day18Ex.txt";
    }

    public override long PartOne()
    {
        var grid = new Grid<int>();
        var last = Pos.Origin;

        foreach (var inst in Input)
        {
            var parts = inst.Spaced().ToArray(3);
            var (dir, len, color) = (parts[0][0], parts[1].AsInt(), Convert.ToInt32(parts[2][2..^1], 16));
            var offset = Pos.RelativeDirection(dir) * len;
            foreach (var pos in last.EachTo(last + offset))
            {
                grid[pos] = color;
            }
            last += offset;
        }

        var inside = false;
        foreach (var pos in grid.Bounds)
        {
            if (grid.Has(pos) && grid.Has(pos + Pos.Down)) inside = !inside;
            else if (inside) grid[pos] = 0;
        }
        
        return grid.Count;
    }
}