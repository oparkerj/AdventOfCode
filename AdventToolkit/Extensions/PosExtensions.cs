using System;
using System.Collections.Generic;
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
    }
}