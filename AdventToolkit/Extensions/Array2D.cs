using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                        yield return new Pos(i, j);
                    }
                }
            }
            else
            {
                for (var j = 0; j < arr.GetLength(1); j++)
                {
                    for (var i = 0; i < arr.GetLength(0); i++)
                    {
                        yield return new Pos(i, j);
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
            return t.All(yInvert).Select(func);
            // return t.Indices(yInvert).Select(pos => func(t.Get(pos)));
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

        public static IEnumerable<IEnumerable<TO>> Select2D<TI, TO>(this IEnumerable<IEnumerable<TI>> ie, Func<TI, TO> func)
        {
            return ie.Select(items => items.Select(func));
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

        public static IEnumerable<(Pos Pos, char Char)> As2D(this IEnumerable<IEnumerable<char>> source)
        {
            var y = 0;
            foreach (var row in source)
            {
                var x = 0;
                foreach (var c in row)
                {
                    yield return (new Pos(x, y), c);
                    x++;
                }
                y++;
            }
        }

        public static string Stringify<T>(this IEnumerable<IEnumerable<T>> source, string rowSep = null, string colSep = null)
        {
            rowSep ??= Environment.NewLine;
            colSep ??= "";
            return string.Join(rowSep, source.Select(row => string.Join(colSep, row)));
        }

        public static string Stringify<T>(this T[,] arr, bool yInvert = false, string rowSep = null, string colSep = null)
        {
            return arr.Stringify(item => item, yInvert, rowSep, colSep);
        }
        
        public static string Stringify<T, TU>(this T[,] arr, Func<T, TU> func, bool yInvert = false, string rowSep = null, string colSep = null)
        {
            rowSep ??= Environment.NewLine;
            colSep ??= "";
            var width = arr.GetLength(0);
            var height = arr.GetLength(1);
            var b = new StringBuilder();
            for (var y = yInvert ? height - 1 : 0; yInvert ? y >= 0 : y < height; y += yInvert ? -1 : 1)
            {
                for (var x = 0; x < width; x++)
                {
                    b.Append(func(arr[x, y]));
                    if (x < width - 1) b.Append(colSep);
                }
                if (yInvert && y > 0 || !yInvert && y < height - 1) b.Append(rowSep);
            }
            return b.ToString();
        }
    }
}