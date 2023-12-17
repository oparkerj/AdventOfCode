using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2023.Puzzles;

public class Day17 : Puzzle<int>
{
    // State includes current position, direction, and straight-line distance
    public record Cart(Pos Current, Pos Dir, int Line);
    
    public override int PartOne()
    {
        var grid = Input.Select2D(c => c.AsInt()).ToGrid();

        // Create a shortest path search with the given constraints
        var dijkstra = new Dijkstra<Cart, Cart>
        {
            Cell = (_, b) => b,
            Distance = state => grid[state.Current],
            Neighbors = state =>
            {
                var (current, dir, line) = state;
                var left = state.Dir.CounterClockwise();
                var right = state.Dir.Clockwise();

                if (Part == 2)
                {
                    if (line < 4)
                    {
                        // Can only go forward
                        return [state with {Current = current + dir, Line = line + 1}];
                    }
                    if (line == 10)
                    {
                        // Must turn
                        return
                        [
                            new Cart(current + left, left, 1),
                            new Cart(current + right, right, 1)
                        ];
                    }
                    // Can go left, right, or forward
                    return
                    [
                        new Cart(current + left, left, 1),
                        new Cart(current + right, right, 1),
                        state with {Current = current + dir, Line = line + 1}
                    ];
                }
                else
                {
                    if (line < 3)
                    {
                        // Can go left, right, or forward
                        return
                        [
                            new Cart(current + left, left, 1),
                            new Cart(current + right, right, 1),
                            state with {Current = current + dir, Line = line + 1}
                        ];
                    }
                    else
                    {
                        // Must turn
                        return
                        [
                            new Cart(current + left, left, 1),
                            new Cart(current + right, right, 1)
                        ];
                    }
                }
            },
        };
        
        // From the start you can go right or down
        var (rightDist, _) = dijkstra.ComputeFind(new Cart(Pos.Origin, Pos.Right, 0), IsAtEnd, state => grid.Has(state.Current));
        var (downDist, _) = dijkstra.ComputeFind(new Cart(Pos.Origin, Pos.Down, 0), IsAtEnd, state => grid.Has(state.Current));
        return Math.Min(rightDist, downDist);

        // If part 2, require minimum distance to reach the end
        bool IsAtEnd(Cart state)
        {
            if (state.Current != grid.Bounds.DiagMaxMin) return false;
            return Part != 2 || state.Line >= 4;
        }
    }
}