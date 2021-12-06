using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using MoreLinq;

namespace AdventToolkit.Extensions
{
    public static class PosExtensions
    {
        public static IEnumerable<Pos> Adjacent(this Pos p, bool yUp = false)
        {
            if (yUp)
            {
                yield return new Pos(p.X, p.Y + 1);
                yield return new Pos(p.X + 1, p.Y);
                yield return new Pos(p.X, p.Y - 1);
                yield return new Pos(p.X - 1, p.Y);
                yield break;
            }
            yield return new Pos(p.X, p.Y - 1);
            yield return new Pos(p.X + 1, p.Y);
            yield return new Pos(p.X, p.Y + 1);
            yield return new Pos(p.X - 1, p.Y);
        }

        public static IEnumerable<Pos> Around(this Pos p)
        {
            yield return new Pos(p.X, p.Y - 1);
            yield return new Pos(p.X + 1, p.Y - 1);
            yield return new Pos(p.X + 1, p.Y);
            yield return new Pos(p.X + 1, p.Y + 1);
            yield return new Pos(p.X, p.Y + 1);
            yield return new Pos(p.X - 1, p.Y + 1);
            yield return new Pos(p.X - 1, p.Y);
            yield return new Pos(p.X - 1, p.Y - 1);
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

        public static IEnumerable<Pos3D> Around(this Pos3D p)
        {
            return Enumerable.Append(Enumerable.Append(p.EnumerateSingle(), p + Pos3D.Forward), p + Pos3D.Backward)
                .SelectMany(pos => Around(pos.To2D).Select(p2 => p2.To3D(pos.Z)));
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
            return Enumerable.Prepend(p.EachBetween(other), p);
        }

        // exclusive at p
        public static IEnumerable<Pos> EachTo(this Pos p, Pos other)
        {
            return Enumerable.Append(p.EachBetween(other), other);
        }

        // inclusive on both ends
        public static IEnumerable<Pos> Connect(this Pos p, Pos other)
        {
            return Enumerable.Append(Enumerable.Prepend(p.EachBetween(other), p), other);
        }

        public static bool AdjacentTo(this Pos p, Pos other)
        {
            return p.MDist(other) == 1;
        }

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

        public static IEnumerable<Pos> Mul(this IEnumerable<Pos> points, int scale)
        {
            return points.Select(pos => pos * scale);
        }
    }
}