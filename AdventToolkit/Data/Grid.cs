using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Data
{
    public class Grid<T> : IEnumerable<KeyValuePair<(int x, int y), T>>
    {
        public readonly Dictionary<(int x, int y), T> Data = new();
        private Rect _bounds;
        public T Default = default; // default value is configurable

        public Rect Bounds
        {
            get => _bounds ??= new Rect();
            set => _bounds = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<(int x, int y), T>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public T this[(int x, int y) p, bool fitBounds = true]
        {
            get => Data.TryGetValue(p, out var t) ? t : Default;
            set
            {
                if (fitBounds) Bounds.Fit(p);
                Data[p] = value;
            }
        }

        public T this[int x, int y]
        {
            get => this[(x, y)];
            set => this[(x, y)] = value;
        }

        public bool Remove((int x, int y) p)
        {
            return Data.Remove(p);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool ContainsValue(T value)
        {
            return Data.ContainsValue(value);
        }

        public (int dx, int dy) GetOffset((int x, int y) p)
        {
            return (p.x - Bounds.MinX, p.y - Bounds.MinY);
        }

        public IEnumerable<T> GetSide(Side side)
        {
            return GetSidePositions(side).Select(pos => this[pos]);
        }
        
        public IEnumerable<Pos> GetSidePositions(Side side)
        {
            var b = Bounds;
            if (side == Side.Top)
            {
                for (var i = b.MinX; i <= b.MaxX; i++)
                {
                    yield return (i, b.MaxY);
                }
            }
            else if (side == Side.Right)
            {
                for (var i = b.MinY; i <= b.MaxY; i++)
                {
                    yield return (b.MaxX, i);
                }
            }
            else if (side == Side.Bottom)
            {
                for (var i = b.MinX; i <= b.MaxX; i++)
                {
                    yield return (i, b.MinY);
                }
            }
            else if (side == Side.Left)
            {
                for (var i = b.MinY; i <= b.MaxY; i++)
                {
                    yield return (b.MinX, i);
                }
            }
        }

        public void ResetBounds()
        {
            Bounds.Initialized = false;
            foreach (var pos in Data.Keys)
            {
                Bounds.Fit(pos);
            }
        }

        public void ForEach(Action<int, int, T> action, GridFilter<T> filter = null)
        {
            ForEach(action, filter, arg => arg);
        }

        public void ForEach<TO>(Action<int, int, TO> action, GridFilter<T> filter = null, Func<T, TO> func = null)
        {
            func ??= _ => default;
            for (var j = Bounds.MinY; j <= Bounds.MaxY; j++)
            {
                for (var i = Bounds.MinX; i <= Bounds.MaxX; i++)
                {
                    var value = Data[(i, j)];
                    var (x, y) = (i, j);
                    var t = value;
                    filter?.Invoke(ref x, ref y, ref t);
                    var o = func(t);
                    action(x, y, o);
                }
            }
        }

        public GridFilter<T> ToIndexFilter(Array array, bool yUp = true)
        {
            return (ref int x, ref int y, ref T _) =>
            {
                x -= Bounds.MinX;
                if (yUp) y = array.GetLength(1) - 1 - (y - Bounds.MinY);
                else y -= Bounds.MinY;
            };
        }

        public T[,] ToArray(bool yUp = true)
        {
            var arr = new T[Bounds.Width, Bounds.Height];
            return ToArray(arr, yUp);
        }

        public T[,] ToArray(T[,] arr, bool yUp = true)
        {
            ForEach((x, y, t) => arr[x, y] = t, ToIndexFilter(arr, yUp));
            return arr;
        }

        public void ToArray<TO>(TO[,] arr, Func<T, TO> func, bool yUp = true)
        {
            ForEach((x, y, o) => arr[x, y] = o, ToIndexFilter(arr, yUp), func);
        }

    }

    public delegate void GridFilter<T>(ref int x, ref int y, ref T t);
    
    public enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }
}