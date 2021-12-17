using System;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using RegExtract;

namespace AdventOfCode2021.Puzzles;

public class Day17 : Puzzle
{
    public Day17()
    {
        Part = 2;
    }

    public Rect GetArea()
    {
        var (a, b, c, d) = InputLine.Extract<(int, int, int, int)>(@".+x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)");
        var x = Interval.RangeInclusive(a, b);
        var y = Interval.RangeInclusive(c, d);
        return new Rect(x, y);
    }

    public override void PartOne()
    {
        var area = GetArea();
        WriteLn(Algorithms.Sum1ToN(-area.MinY - 1));
    }

    public override void PartTwo()
    {
        var area = GetArea();
        var maxY = Algorithms.Sum1ToN(-area.MinY - 1);
        // Manually check velocities within plausible extremes
        var velocityRange = new Rect(new Pos((area.MinX * 2).SqrtFloor(), area.MinY), new Pos(area.MaxX, maxY));
        var count = velocityRange.Count(vel =>
        {
            var t = Pos.Zero;
            while (t.X <= area.MaxX && t.Y >= area.MinY)
            {
                t += vel;
                if (area.Contains(t)) return true;
                vel = new Pos(vel.X - Math.Sign(vel.X), vel.Y - 1);
            }
            return false;
        });
        WriteLn(count);
    }
}