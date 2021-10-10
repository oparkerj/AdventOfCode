using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Space
{
    public abstract class GridBase<T> : AlignedSpace<Pos, T>
    {
        private Rect _bounds;
        
        public bool IncludeCorners { get; }

        public bool FitBounds { get; set; } = true;

        protected GridBase() : this(false) { }

        protected GridBase(bool includeCorners = false)
        {
            IncludeCorners = includeCorners;
        }

        protected GridBase(GridBase<T> other)
        {
            CopyFrom(other);
        }

        protected void CopyFrom(GridBase<T> other)
        {
            _bounds = other._bounds == null ? null : new Rect(other._bounds);
            foreach (var (pos, value) in other)
            {
                this[pos] = value;
            }
        }

        public Rect Bounds
        {
            get => _bounds ??= new Rect();
            set => _bounds = value;
        }

        public override T this[Pos pos]
        {
            get => base[pos];
            set
            {
                if (FitBounds) Bounds.Fit(pos);
                base[pos] = value;
            }
        }

        public T this[int x, int y]
        {
            get => this[new Pos(x, y)];
            set => this[new Pos(x, y)] = value;
        }

        public override IEnumerable<Pos> GetNeighbors(Pos pos)
        {
            return IncludeCorners ? pos.Around() : pos.Adjacent();
        }
        
        public IEnumerable<T> GetSide(Side side, Rect rect = null)
        {
            rect ??= Bounds;
            return rect.GetSidePositions(side).Select(pos => this[pos]);
        }

        public void ResetBounds()
        {
            Bounds.Rebound(Positions);
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
}