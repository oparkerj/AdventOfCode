using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day14 : Puzzle<int>
{
    int GetLoad(Grid<char> grid)
    {
        var yStart = grid.Bounds.MinY;
        return grid.WhereValue('O').Select(pair => pair.Key.Y - yStart + 1).Sum();
    }
    
    void TiltGrid(Grid<char> grid, Pos direction)
    {
        var range = direction switch
        {
            (0, 1) => grid.Bounds.YRange.Reverse(),
            (0, -1) => grid.Bounds.YRange,
            (1, 0) => grid.Bounds.XRange.Reverse(),
            (-1, 0) => grid.Bounds.XRange,
        };
            
        foreach (var line in range)
        {
            foreach (var rock in GetPositions(line))
            {
                var hit = rock.Trace(direction, pos => grid[pos] != '.');
                hit -= direction;
                grid[rock] = '.';
                grid[hit] = 'O';
            }
        }
        return;

        IEnumerable<Pos> GetPositions(int current)
        {
            if (direction.X == 0) return grid.Bounds.XRange.Select(i => new Pos(i, current)).Where(pos => grid[pos] == 'O');
            return grid.Bounds.YRange.Select(i => new Pos(current, i)).Where(pos => grid[pos] == 'O');
        }
    }

    public override int PartOne()
    {
        var grid = Input.ToGrid();
        TiltGrid(grid, Pos.Up);
        return GetLoad(grid);
    }

    public override int PartTwo()
    {
        var grid = Input.ToGrid();
        var (offset, cycle) = Algorithms.FindCyclePeriod(grid, g => (g.ToArray().Stringify(), GetLoad(g)), CycleGrid);
        var times = 1_000_000_000L.CycleOffset(offset, cycle);
        
        // Simulate again to the offset that represents 1 billion cycles
        grid = Input.ToGrid();
        grid.RepeatAction(CycleGrid, times);
        return GetLoad(grid);

        void CycleGrid(Grid<char> g)
        {
            TiltGrid(g, Pos.Up);
            TiltGrid(g, Pos.Left);
            TiltGrid(g, Pos.Down);
            TiltGrid(g, Pos.Right);
        }
    }
}