using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using MoreLinq;

namespace AdventToolkit.Extensions;

public static class PosExtensions
{
    public static IEnumerable<Pos> Adjacent(this Pos p)
    {
        yield return new Pos(p.X, p.Y - 1);
        yield return new Pos(p.X + 1, p.Y);
        yield return new Pos(p.X, p.Y + 1);
        yield return new Pos(p.X - 1, p.Y);
    }

    public static IEnumerable<Pos> Around(this Pos p)
    {
        yield return new Pos(p.X - 1, p.Y + 1);
        yield return new Pos(p.X, p.Y + 1);
        yield return new Pos(p.X + 1, p.Y + 1);
        yield return new Pos(p.X - 1, p.Y);
        yield return new Pos(p.X + 1, p.Y);
        yield return new Pos(p.X - 1, p.Y - 1);
        yield return new Pos(p.X, p.Y - 1);
        yield return new Pos(p.X + 1, p.Y - 1);
    }

    // Get the rectangle of positions around a specified position.
    // The parameter specifies how far to extend the rectangle in each direction.
    // For example, setting leftRight to 1 adds 1 to both the left and right side for a width of 3.
    public static IEnumerable<Pos> RectAround(this Pos p, int leftRight, int upDown)
    {
        var xx = p.X + leftRight;
        for (int y = p.Y + upDown, yy = p.Y - upDown; y >= yy; y--)
        {
            for (var x = p.X - leftRight; x <= xx; x++)
            {
                yield return new Pos(x, y);
            }
        }
    }

    public static IEnumerable<Pos> Corners(this Pos p)
    {
        yield return new Pos(p.X + 1, p.Y + 1);
        yield return new Pos(p.X - 1, p.Y + 1);
        yield return new Pos(p.X + 1, p.Y - 1);
        yield return new Pos(p.X - 1, p.Y - 1);
    }

    public static Pos GetSide(this Pos p, Side side)
    {
        return side switch
        {
            Side.Top => p + Pos.Up,
            Side.Right => p + Pos.Right,
            Side.Bottom => p + Pos.Down,
            Side.Left => p + Pos.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }
    
    public static Side ToSide(this Pos dir)
    {
        return dir switch
        {
            (0, 1) => Side.Top,
            (1, 0) => Side.Right,
            (0, -1) => Side.Bottom,
            (-1, 0) => Side.Left
        };
    }
    
    public static Pos ToSide(this Side side)
    {
        return side switch
        {
            Side.Bottom => Pos.Down,
            Side.Left => Pos.Left,
            Side.Right => Pos.Right,
            Side.Top => Pos.Up,
        };
    }

    public static IEnumerable<Pos3D> Around(this Pos3D p)
    {
        foreach (var pos in p.To2D.Around())
        {
            yield return pos.To3D(p.Z - 1);
            yield return pos.To3D(p.Z);
            yield return pos.To3D(p.Z + 1);
        }
    }

    public static IEnumerable<Pos3D> Adjacent(this Pos3D p)
    {
        return Adjacent(p.To2D).Select(p2 => p2.To3D(p.Z)).Then(p + Pos3D.Forward).Then(p + Pos3D.Backward);
    }

    public static IEnumerable<Pos> ToPositions(this IEnumerable<int[]> source)
    {
        foreach (var ints in source)
        {
            if (ints.Length < 2) throw new Exception("Invalid array size.");
            yield return new Pos(ints[0], ints[1]);
        }
    }

    public static IEnumerable<Pos3D> ToPositions3D(this IEnumerable<int[]> source, int? z = null)
    {
        if (z == null)
        {
            foreach (var ints in source)
            {
                if (ints.Length < 3) throw new Exception("Invalid array size.");
                yield return new Pos3D(ints[0], ints[1], ints[2]);
            }
        }
        else
        {
            var depth = z.Value;
            foreach (var ints in source)
            {
                if (ints.Length < 2) throw new Exception("Invalid array size.");
                yield return new Pos3D(ints[0], ints[1], depth);
            }
        }
    }

    // exclusive on both ends
    public static IEnumerable<Pos> EachBetween(this Pos p, Pos other)
    {
        var delta = p.Towards(other);
        p += delta;
        while (p != other)
        {
            yield return p;
            p += delta;
        }
    }

    // exclusive at other
    public static IEnumerable<Pos> FromTo(this Pos p, Pos other)
    {
        return p.EachBetween(other).Before(p);
    }

    // exclusive at p
    public static IEnumerable<Pos> EachTo(this Pos p, Pos other)
    {
        return p.EachBetween(other).Then(other);
    }

    // inclusive on both ends
    public static IEnumerable<Pos> Connect(this Pos p, Pos other)
    {
        return p.EachBetween(other).Before(p).Then(other);
    }

    public static bool AdjacentTo(this Pos p, Pos other)
    {
        return p.MDist(other) == 1;
    }

    // Convert a series of delta offsets to absolute offsets
    public static IEnumerable<Pos> MakePath(this IEnumerable<Pos> deltas, Pos start)
    {
        return deltas.Scan(start, Pos.Add);
    }

    public static IEnumerable<Pos> MakePath(this Pos start, IEnumerable<Pos> deltas)
    {
        return deltas.MakePath(start);
    }

    // Connect a series of points made of vertical or horizontal lines.
    // The first point itself is not included.
    public static IEnumerable<Pos> ConnectLines(this IEnumerable<Pos> points)
    {
        return points.Pairwise(EachTo).Flatten();
    }
    
    public static IEnumerable<Pos> ConnectLinesAll(this IEnumerable<Pos> points)
    {
        var items = points.AsList();
        return items.Pairwise(EachTo).Flatten().Before(items[0]);
    }

    // Removes intermediate points that are traveling in the same direction.
    // Returns only the corners that make up the path.
    // This is the inverse of ConnectLinesAll.
    public static IEnumerable<Pos> SimplifyPath(this IEnumerable<Pos> points)
    {
        using var e = points.GetEnumerator();
        if (!e.MoveNext()) yield break;
        var current = e.Current;
        yield return current;
        if (!e.MoveNext()) yield break;
        var dir = current.Towards(e.Current);
        current = e.Current;
        while (e.MoveNext())
        {
            var next = current.Towards(e.Current);
            if (next != dir)
            {
                yield return current;
                dir = next;
            }
            current = e.Current;
        }
        yield return current;
    }

    public static IEnumerable<Pos> Mul(this IEnumerable<Pos> points, int scale)
    {
        return points.Select(pos => pos * scale);
    }

    public static Pos SumWithin(this Pos p, Pos other, Rect r)
    {
        var target = p + other;
        if (!r.Contains(p)) return target;
        return r.Contains(target) ? target : p;
    }

    public static Pos Abs(this Pos p) => new(Math.Abs(p.X), Math.Abs(p.Y));
    
    public static IEnumerable<Pos> GetMDistRing(this Pos pos, int range)
    {
        return Pos.Directions.Select(dir => dir * range + pos)
            .RepeatAmount(1)
            .ConnectLines();
    }

    public static IEnumerable<Pos> GetMDistFill(this Pos pos, int range)
    {
        return Enumerable.Range(1, range)
            .Select(i => pos.GetMDistRing(i))
            .Flatten()
            .Before(pos);
    }

    public static IEnumerable<Pos> Shift(this IEnumerable<Pos> points, Pos offset)
    {
        return points.Select(pos => pos + offset);
    }
}