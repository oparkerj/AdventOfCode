using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day10 : Puzzle<int>
{
    public const string PipeTypes = "|-LJ7F";
    
    [Flags]
    public enum Side
    {
        None = 0,
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,
    }

    public readonly record struct Pipe(Side Sides = Side.None)
    {
        public IEnumerable<Pos> Adjacent()
        {
            if (Sides.HasFlag(Side.Up)) yield return Pos.Up;
            if (Sides.HasFlag(Side.Right)) yield return Pos.Right;
            if (Sides.HasFlag(Side.Down)) yield return Pos.Down;
            if (Sides.HasFlag(Side.Left)) yield return Pos.Left;
        }
    }

    public Pipe GetPipe(char c)
    {
        return c switch
        {
            '|' => new Pipe(Side.Up | Side.Down),
            '-' => new Pipe(Side.Left | Side.Right),
            'L' => new Pipe(Side.Up | Side.Right),
            'J' => new Pipe(Side.Up | Side.Left),
            '7' => new Pipe(Side.Down | Side.Left),
            'F' => new Pipe(Side.Down | Side.Right),
            _ => new Pipe(),
        };
    }

    public Grid<Pipe> FindLoop(out int max)
    {
        var chars = Input.ToGrid();
        var grid = new Grid<Pipe>();
        var queue = new Queue<(Pos, int)>();
        
        var start = chars.Find('S');
        // Find which pipes connect to the starting position
        foreach (var pos in start.Adjacent())
        {
            var pipe = GetPipe(chars[pos]);
            foreach (var adj in pipe.Adjacent())
            {
                if (pos + adj == start)
                {
                    grid[pos] = pipe;
                    queue.Enqueue((pos, 1));
                }
            }
        }
        
        // Figure out what kind of pipe the start position is
        var startAdjacent = grid.Positions.ToHashSet();
        foreach (var c in PipeTypes)
        {
            var possible = GetPipe(c);
            var possibleAdjacent = possible.Adjacent().Select(pos => start + pos).ToHashSet();
            if (startAdjacent.SetEquals(possibleAdjacent))
            {
                grid[start] = possible;
                break;
            }
        }

        max = 0;

        // Explore the pipes
        while (queue.Count > 0)
        {
            var (next, dist) = queue.Dequeue();
            max = Math.Max(max, dist);
            foreach (var off in grid[next].Adjacent())
            {
                var adj = next + off;
                if (grid.Has(adj)) continue;
                var nextPipe = GetPipe(chars[adj]);
                grid[adj] = nextPipe;
                queue.Enqueue((adj, dist + 1));
            }
        }

        return grid;
    }

    public override int PartOne()
    {
        _ = FindLoop(out var result);
        return result;
    }

    public override int PartTwo()
    {
        var grid = FindLoop(out _);

        var bound = grid.Bounds;
        var check = new HashSet<Pos>(bound);

        var inside = 0;
        foreach (var pos in check)
        {
            if (grid.Has(pos)) continue;
            var ups = 0;
            var downs = 0;
            
            var current = pos;
            while (bound.Contains(current))
            {
                current += Pos.Left;
                var pipe = grid[current];
                if (pipe.Sides.HasFlag(Side.Up))
                {
                    ups++;
                }
                if (pipe.Sides.HasFlag(Side.Down))
                {
                    downs++;
                }
                
                // After exiting a horizontal line of pipe, check if we actually
                // crossed the pipe or just grazed it.
                if (pipe.Sides.HasFlag(Side.Right) && !pipe.Sides.HasFlag(Side.Left))
                {
                    if (ups != downs)
                    {
                        var min = Math.Min(ups, downs);
                        (ups, downs) = (min, min);
                    }
                }
            }

            if (((ups + downs) / 2) % 2 == 1)
            {
                inside++;
            }
        }
        
        return inside;
    }
}