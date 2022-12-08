using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day8 : Puzzle<int>
{
    public override int PartOne()
    {
        var grid = Input.ToGrid();
        var count = 0;

        foreach (var check in grid.Positions)
        {
            var height = grid[check];
            var visible = false;
            foreach (var dir in Pos.Directions())
            {
                var hit = check.Trace(dir, pos => !grid.Has(pos) || grid[pos] >= height);
                if (!grid.Has(hit)) visible = true;
            }
            if (visible) count++;
        }

        return count;
    }

    public override int PartTwo()
    {
        var grid = Input.ToGrid();

        int Score(Pos check)
        {
            var total = 1;
            var height = grid[check];
            foreach (var dir in Pos.Directions())
            {
                var hit = check.Trace(dir, pos => !grid.Has(pos) || grid[pos] >= height);
                if (!grid.Has(hit)) total *= check.MDist(hit) - 1;
                else total *= check.MDist(hit);
            }
            return total;
        }
        
        return grid.Positions.Max(Score);
    }
}