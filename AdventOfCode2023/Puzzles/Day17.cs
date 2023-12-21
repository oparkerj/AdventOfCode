using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2023.Puzzles;

public class Day17 : Puzzle<int>
{
    // State includes current position, direction, and straight-line distance
    public record Cart(Pos Current, Pos Dir, int Line)
    {
        public Cart Move(Pos dir) => new(Current + dir, dir, Dir == dir ? Line + 1 : 1);

        public Cart Forward() => Move(Dir);

        public Cart Left() => Move(Dir.CounterClockwise());
        
        public Cart Right() => Move(Dir.Clockwise());
    }
    
    public override int PartOne()
    {
        var grid = Input.Select2D(c => c.AsInt()).ToGrid();

        // Create a shortest path search with the given constraints
        var dijkstra = new Dijkstra<Cart, Cart>
        {
            Cell = (_, b) => b,
            Distance = state => grid[state.Current],
            Neighbors = NextCarts,
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

    private IEnumerable<Cart> NextCarts(Cart cart)
    {
        var straight = cart.Line;

        if (Part == 2)
        {
            if (straight < 4) return [cart.Forward()];
            if (straight == 10) return [cart.Left(), cart.Right()];
            return [cart.Left(), cart.Right(), cart.Forward()];
        }
        
        if (straight < 3) return [cart.Left(), cart.Right(), cart.Forward()];
        return [cart.Left(), cart.Right()];
    }
}