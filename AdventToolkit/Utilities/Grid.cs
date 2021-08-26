using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventToolkit.Utilities
{
    public class Grid<T> : AlignedSpace<Pos, T>
    {
        private Rect _bounds;
        private bool _includeCorners;
        
        public Grid() : this(false) { }

        public Grid(bool includeCorners = false)
        {
            _includeCorners = includeCorners;
        }

        public Grid(Grid<T> other) : this(other._includeCorners)
        {
            _bounds = other._bounds == null ? null : new Rect(other._bounds);
            foreach (var (pos, value) in other)
            {
                this[pos] = value;
            }
        }

        public static Func<Pos, IEnumerable<Pos>> Adjacent() => pos => pos.Adjacent();

        public static Func<Pos, IEnumerable<Pos>> Around() => pos => pos.Around();

        public Rect Bounds
        {
            get => _bounds ??= new Rect();
            set => _bounds = value;
        }

        public override T this[Pos p, bool fitBounds = true]
        {
            get => base[p, fitBounds];
            set
            {
                if (fitBounds) Bounds.Fit(p);
                base[p, fitBounds] = value;
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

        public IEnumerable<T> GetSide(Side side, Rect rect = null)
        {
            rect ??= Bounds;
            return rect.GetSidePositions(side).Select(pos => this[pos]);
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

    public static class GridExtensions
    {
        public static TGrid ToGrid<T, TGrid>(this IEnumerable<IEnumerable<T>> source, bool decreaseY = true)
            where TGrid : Grid<T>, new()
        {
            var grid = new TGrid();
            var y = 0;
            foreach (var row in source)
            {
                var x = 0;
                foreach (var t in row)
                {
                    grid[x, y] = t;
                    x++;
                }
                if (decreaseY) y--;
                else y++;
            }
            return grid;
        }
        
        public static Grid<T> ToGrid<T>(this IEnumerable<IEnumerable<T>> source, bool decreaseY = true)
        {
            return source.ToGrid<T, Grid<T>>(decreaseY);
        }

        public static Grid<bool> ToGrid(this IEnumerable<Pos> source)
        {
            var grid = new Grid<bool>();
            foreach (var pos in source)
            {
                grid[pos] = true;
            }
            return grid;
        }

        public static Grid<T> ToGridRows<T>(this IEnumerable<T> source, int width, bool decreaseY = false)
        {
            var grid = new Grid<T>();
            foreach (var (y, row) in source.Batch(width).Index())
            {
                foreach (var (x, item) in row.Index())
                {
                    grid[x, y] = item;
                }
            }
            if (decreaseY) SimpleGridTransformer<T>.FlipV.ApplyTo(grid);
            return grid;
        }
    }
}