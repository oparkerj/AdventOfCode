using System;
using System.Collections.Generic;
using AdventToolkit.Data;

namespace AdventToolkit.Extensions
{
    public static class Array2D
    {
        public static IEnumerable<T> All<T>(this T[,] t)
        {
            for (var i = 0; i < t.GetLength(0); i++)
            {
                for (var j = 0; j < t.GetLength(1); j++)
                {
                    yield return t[i, j];
                }
            }
        }

        public static bool Has<T>(this T[,] t, Pos p)
        {
            var w = t.GetLength(0);
            var h = t.GetLength(1);
            return p.X >= 0 && p.X < w && p.Y >= 0 && p.Y < h;
        }

        public static T Get<T>(this T[,] t, Pos p)
        {
            return t[p.X, p.Y];
        }
        
        public static T GetOrDefault<T>(this T[,] t, Pos p, T def = default)
        {
            return t.Has(p) ? t[p.X, p.Y] : def;
        }
        
        public static IEnumerable<Pos> Adjacent(this Pos p)
        {
            yield return (p.X, p.Y - 1);
            yield return (p.X + 1, p.Y);
            yield return (p.X, p.Y + 1);
            yield return (p.X - 1, p.Y);
        }

        public static IEnumerable<Pos> Adjacent(this (int x, int y) p)
        {
            return Adjacent((Pos) p);
        }

        public static IEnumerable<Pos> Around(this Pos p)
        {
            yield return (p.X, p.Y - 1);
            yield return (p.X + 1, p.Y - 1);
            yield return (p.X + 1, p.Y);
            yield return (p.X + 1, p.Y + 1);
            yield return (p.X, p.Y + 1);
            yield return (p.X - 1, p.Y + 1);
            yield return (p.X - 1, p.Y);
            yield return (p.X - 1, p.Y - 1);
        }

        public static IEnumerable<Pos> Around(this (int x, int y) p)
        {
            return Around((Pos) p);
        }

        public static int MDist(this (int x, int y) p, (int x, int y) other)
        {
            return Math.Abs(p.x - other.x) + Math.Abs(p.y - other.y);
        }

        public static Pos Clockwise(this (int x, int y) p, Pos center = default)
        {
            return (p.y - center.Y + center.X, center.X - p.x + center.Y);
        }
        
        public static Pos CounterClockwise(this (int x, int y) p, Pos center = default)
        {
            return (center.Y - p.y + center.X, p.x - center.X + center.Y);
        }

        public static bool Contains(this (int a, int b) range, int i, bool inclusive = false)
        {
            if (inclusive) return i >= range.a && i <= range.b;
            return i >= range.a && i < range.b;
        }
    }
}