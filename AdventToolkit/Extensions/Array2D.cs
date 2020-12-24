using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class Array2D
    {
        public static IEnumerable<Pos> Indices<T>(this T[,] arr, bool yInvert = false)
        {
            if (yInvert)
            {
                for (var j = arr.GetLength(1) - 1; j >= 0; j--)
                {
                    for (var i = 0; i < arr.GetLength(0); i++)
                    {
                        yield return (i, j);
                    }
                }
            }
            else
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    for (var i = 0; i < arr.GetLength(0); i++)
                    {
                        yield return (i, j);
                    }
                }
            }
        }
        
        public static IEnumerable<T> All<T>(this T[,] t, bool yInvert = false)
        {
            return t.Indices(yInvert).Select(t.Get);
        }
        
        public static IEnumerable<TU> All<T, TU>(this T[,] t, Func<T, TU> func, bool yInvert = false)
        {
            return t.Indices(yInvert).Select(pos => func(t.Get(pos)));
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
        
        public static IEnumerable<Pos> Adjacent(this Pos p, bool yUp = false)
        {
            if (yUp)
            {
                yield return (p.X, p.Y + 1);
                yield return (p.X + 1, p.Y);
                yield return (p.X, p.Y - 1);
                yield return (p.X - 1, p.Y);
                yield break;
            }
            yield return (p.X, p.Y - 1);
            yield return (p.X + 1, p.Y);
            yield return (p.X, p.Y + 1);
            yield return (p.X - 1, p.Y);
        }

        public static IEnumerable<Pos> Adjacent(this (int x, int y) p, bool yUp = false)
        {
            return Adjacent((Pos) p, yUp);
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

        public static IEnumerable<T> Flatten<T>(this T[,] arr, T sep)
        {
            for (var j = arr.GetLength(1) - 1; j >= 0; j--)
            {
                for (var i = 0; i < arr.GetLength(0); i++)
                {
                    yield return arr[i, j];
                }
                if (j > 0) yield return sep;
            }
        }
        
        public static IEnumerable<TU> Flatten<T, TU>(this T[,] arr, Func<T, TU> func, TU sep)
        {
            for (var j = arr.GetLength(1) - 1; j >= 0; j--)
            {
                for (var i = 0; i < arr.GetLength(0); i++)
                {
                    yield return func(arr[i, j]);
                }
                if (j > 0) yield return sep;
            }
        }

        public static IEnumerable<((int x, int y) Pos, char Char)> As2D(this IEnumerable<IEnumerable<char>> source)
        {
            var y = 0;
            foreach (var row in source)
            {
                var x = 0;
                foreach (var c in row)
                {
                    yield return ((x, y), c);
                    x++;
                }
                y++;
            }
        }

        public static Pos Trace(this Pos start, Pos dir, Func<(int x, int y), bool> func)
        {
            var (x, y) = start;
            var (dx, dy) = dir;
            while (true)
            {
                x += dx;
                y += dy;
                if (func((x, y))) break;
            }
            return (x, y);
        }

        public static (int x, int y) Trace(this (int x, int y) start, (int x, int y) dir, Func<(int x, int y), bool> func)
        {
            return ((Pos) start).Trace(dir, func);
        }
    }
}