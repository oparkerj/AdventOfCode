using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

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
            return p.EnumerateSingle()
                .Append(p + Pos3D.Forward)
                .Append(p + Pos3D.Backward)
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
    }
}