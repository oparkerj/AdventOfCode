using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day18 : Puzzle<int>
{
    public override int PartOne()
    {
        var points = new HashSet<Pos3D>();
        foreach (var pos in Input.Select(Pos3D.Parse))
        {
            points.Add(pos);
        }
        return points.Sum(point => point.Adjacent().Count(p => !points.Contains(p)));
    }

    public override int PartTwo()
    {
        var points = new Blocks<bool>();

        foreach (var pos in Input.Select(Pos3D.Parse))
        {
            points[pos] = true;
        }

        var x = new Interval();
        var y = new Interval();
        var z = new Interval();

        foreach (var p in points.Positions)
        {
            x = x.Fit(p.X);
            y = y.Fit(p.Y);
            z = z.Fit(p.Z);
        }

        x = x.Expand(1, 1);
        y = y.Expand(1, 1);
        z = z.Expand(1, 1);

        var start = new Pos3D(x.Start, y.Start, z.Start);
        var result = 0;
        
        // BFS, add one to the count if we visit a block from a new direction
        var visited = new HashSet<(Pos3D Block, Pos3D From)>();
        var queue = new Queue<Pos3D>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            var near = points.GetNeighbors(pos).Where(p => !visited.Contains((p, pos)));
            foreach (var next in near)
            {
                if (!(x.Contains(next.X) && y.Contains(next.Y) && z.Contains(next.Z))) continue;
                if (points.Has(next)) result++;
                else queue.Enqueue(next);
                visited.Add((next, pos));
            }
        }
        
        return result;
    }
}