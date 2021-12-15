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
        var target = map.Bounds.Max;
        var paths = map.ToDijkstraWeights().ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        WriteLn(sum);
    }

    public override void PartTwo()
    {
        var map = Input.Select2D(c => c.AsInt()).ToGrid(false);
        var bounds = new Rect(map.Bounds);

        foreach (var pos in new Rect(5, 5).Without(Pos.Origin))
        {
            var (x, y) = pos;
            var view = map.View(new Rect(bounds.Width * x, bounds.Height * y, bounds.Width, bounds.Height));
            view.OffsetX = -bounds.Width * x;
            view.OffsetY = -bounds.Height * y;
            view.OverlayTransformed((_, part) => ((part + pos.MDist(Pos.Origin)) - 1) % 9 + 1);
        }
        
        var target = map.Bounds.Max;
        var paths = map.ToDijkstraWeights().ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        WriteLn(sum);
    }
}