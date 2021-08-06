using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class Grid<T> : AlignedSpace<Pos, T>
    {
        private Rect _bounds;
        private bool _includeCorners;

        public Grid(bool includeCorners = false)
        {
            _includeCorners = includeCorners;
        }

        public static Func<Pos, IEnumerable<Pos>> Adjacent() => pos => pos.Adjacent();

        public static Func<Pos, IEnumerable<Pos>> Around() => pos => pos.Around();

        public Rect Bounds
        {
            get => _bounds ??= new Rect();
            set => _bounds = value;
        }

        public T this[Pos p, bool fitBounds = true]
        {
            get => base[p];
            set
            {
                if (fitBounds) Bounds.Fit(p);
                base[p] = value;
            }
        }

        public T this[int x, int y]
        {
            get => this[new Pos(x, y)];
            set => this[new Pos(x, y)] = value;
        }

        public override IEnumerable<Pos> GetNeighbors(Pos pos)
        {
            return _includeCorners ? pos.Around() : pos.Adjacent();
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

        // Re-align bounding box
        public void ResetBounds()
        {
            Bounds.Initialized = false;
            foreach (var pos in Points.Keys)
            {
                Bounds.Fit(pos);
            }
        }

        public SpaceExtensions.SpaceFilter<Pos, T> ToIndexFilter(Array array, bool yUp = true)
        {
            return (ref Pos pos, ref T _) =>
            {
                var (x, y) = pos;
                x -= Bounds.MinX;
                if (yUp) y = array.GetLength(1) - 1 - (y - Bounds.MinY);
                else y -= Bounds.MinY;
                pos = new Pos(x, y);
            };
        }

        public T[,] ToArray(bool yUp = true)
        {
            var arr = new T[Bounds.Width, Bounds.Height];
            return ToArray(arr, yUp);
        }

        public T[,] ToArray(T[,] arr, bool yUp = true)
        {
            foreach (var ((x, y), val) in this.Select(ToIndexFilter(arr, yUp)))
            {
                arr[x, y] = val;
            }
            return arr;
        }
    }

    public enum Side
    {
        Top,
        Right,
        Bottom,
        Left
    }
}