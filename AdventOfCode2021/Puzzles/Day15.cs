using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Utilities;

namespace AdventOfCode2021.Puzzles;

public class Day15 : Puzzle
{
    public Day15()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var map = Input.Select2D(c => c.AsInt()).ToGrid(false);
        var bounds = map.Bounds;
        var dijkstra = new Dijkstra<Pos, (Pos, int)>
        {
            Neighbors = pos => map.GetNeighbors(pos).Where(bounds.Contains).Select(p => (p, map[p])),
            Distance = tuple => tuple.Item2,
            Cell = (_, tuple) => tuple.Item1
        };
        var target = map.Bounds.Max;
        var paths = dijkstra.ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        Clip(sum);
    }

    public override void PartTwo()
    {
        var map = Input.Select2D(c => c.AsInt()).ToGrid(false);
        var bounds = new Rect(map.Bounds);

        foreach (var pos in new Rect(5, 5).Without(Pos.Origin))
        {
            var (x, y) = pos;
            var window = map.View(new Rect(bounds.Width * x, bounds.Height * y, bounds.Width, bounds.Height));
            window.OffsetX = -bounds.Width * x;
            window.OffsetY = -bounds.Height * y;
            window.OverlayTransformed((_, part) => ((part + pos.MDist(Pos.Origin)) - 1) % 9 + 1);
        }

        bounds = map.Bounds;
        var dijkstra = new Dijkstra<Pos, (Pos, int)>
        {
            Neighbors = pos => map.GetNeighbors(pos).Where(bounds.Contains).Select(p => (p, map[p])),
            Distance = tuple => tuple.Item2,
            Cell = (_, tuple) => tuple.Item1
        };
        var target = bounds.Max;
        var paths = dijkstra.ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        Clip(sum);
    }
}