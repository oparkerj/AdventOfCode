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
        var map = Input.Digits().ToGrid(false);
        var target = map.Bounds.Max;
        var paths = map.ToDijkstraWeights().ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        WriteLn(sum);
    }

    public override void PartTwo()
    {
        var map = Input.Digits().ToGrid(false);
        var bounds = new Rect(map.Bounds);

        foreach (var pos in new Rect(5, 5).Without(Pos.Origin))
        {
            var view = map.View(new Rect(bounds.Size * pos, bounds.Width, bounds.Height));
            view.Offset = -bounds.Size * pos;
            view.OverlayTransformed((_, part) => (part + pos.MDist(Pos.Origin)).ModRange(1..10));
        }
        
        var target = map.Bounds.Max;
        var paths = map.ToDijkstraWeights().ComputePath(Pos.Origin, target);
        var sum = paths.GetPathTo(target).Select(pos => map[pos]).Sum() - map[Pos.Origin];
        WriteLn(sum);
    }
}