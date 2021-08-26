using System;
using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class Rect
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Initialized;

        public Rect() { }

        public Rect(int minX, int minY, int width, int height)
        {
            MinX = minX;
            MinY = minY;
            Width = width;
            Height = height;
        }

        public Rect(Pos a, Pos b)
        {
            var min = a.Min(b);
            (MinX, MinY) = min;
            var (x, y) = a.Max(b);
            Width = x - MinX + 1;
            Height = y - MinY + 1;
        }

        public Rect(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Rect(Rect other) : this(other.MinX, other.MinY, other.Width, other.Height) { }

        public static Rect Bound(IEnumerable<Pos> points)
        {
            var rect = new Rect();
            foreach (var point in points)
            {
                rect.Fit(point);
            }
            return rect;
        }

        public void Deconstruct(out int minX, out int minY, out int width, out int height)
        {
            minX = MinX;
            minY = MinY;
            width = Width;
            height = Height;
        }

        public void Deconstruct(out Pos min, out Pos max)
        {
            min = Min;
            max = Max;
        }
        
        public int Area => Width * Height;

        public long LongArea => (long) Width * Height;

        public int MaxX
        {
            get => MinX + Width - 1;
            set
            {
                Width = value - MinX + 1;
                if (Width < 0) throw new ArgumentException("Rect resized to negative width.");
            }
        }

        public int MaxY
        {
            get => MinY + Height - 1;
            set
            {
                Height = value - MinY + 1;
                if (Height < 0) throw new ArgumentException("Rect resized to negative height.");
            }
        }

        public double MidX => (MinX + MaxX) / 2d;

        public double MidY => (MinY + MaxY) / 2d;

        public Pos MidPos => new((MinX + MaxX) / 2, (MinY + MaxY) / 2);

        public Pos Min => new(MinX, MinY);

        public Pos Max => new(MaxX, MaxY);

        public bool IsEmpty => Width == 0 || Height == 0;

        public bool NonEmpty => !IsEmpty;

        public Rect Fit(Pos p)
        {
            var set = !Initialized;
            if (p.X < MinX || set) MinX = p.X;
            if (p.X > MaxX || set) MaxX = p.X;
            if (p.Y < MinY || set) MinY = p.Y;
            if (p.Y > MaxY || set) MaxY = p.Y;
            Initialized = true;
            return this;
        }

        public void Rebound(IEnumerable<Pos> points)
        {
            Initialized = false;
            foreach (var point in points)
            {
                Fit(point);
            }
        }

        public Rect Intersection(Rect other)
        {
            var x = new Interval(MinX, Width).Overlap(new Interval(other.MinX, other.Width));
            var y = new Interval(MinY, Height).Overlap(new Interval(other.MinY, other.Height));
            if (x.Length == 0 || y.Length == 0) return new Rect();
            return new Rect(x.Start, y.Start, x.Length, y.Length);
        }

        public IEnumerable<Pos> Corners()
        {
            if (IsEmpty) yield break;
            yield return new Pos(MinX, MinY);
            yield return new Pos(MaxX, MinY);
            yield return new Pos(MaxX, MaxY);
            yield return new Pos(MinX, MaxY);
        }

        public IEnumerable<Pos> Positions()
        {
            if (IsEmpty) yield break;
            for (var y = MinY; y <= MaxY; y++)
            {
                for (var x = MinX; x <= MaxX; x++)
                {
                    yield return new Pos(x, y);
                }
            }
        }
        
        public bool OnSide(Pos pos)
        {
            return pos.X == MinX || pos.X == MaxX && pos.Y == MinY || pos.Y == MaxY;
        }

        public IEnumerable<Pos> GetAllSides()
        {
            // Top
            for (var i = MinX; i < MaxX; i++)
            {
                yield return (i, MaxY);
            }
            // Right
            for (var i = MinY + 1; i <= MaxY; i++)
            {
                yield return (MaxX, i);
            }
            // Bottom
            for (var i = MinX + 1; i <= MaxX; i++)
            {
                yield return (i, MinY);
            }
            // Left
            for (var i = MinY; i < MaxY; i++)
            {
                yield return (MinX, i);
            }
        }

        public IEnumerable<Pos> GetSidePositions(Side side)
        {
            if (IsEmpty) yield break;
            if (side == Side.Top)
            {
                for (var i = MinX; i <= MaxX; i++)
                {
                    yield return (i, MaxY);
                }
            }
            else if (side == Side.Right)
            {
                for (var i = MinY; i <= MaxY; i++)
                {
                    yield return (MaxX, i);
                }
            }
            else if (side == Side.Bottom)
            {
                for (var i = MinX; i <= MaxX; i++)
                {
                    yield return (i, MinY);
                }
            }
            else if (side == Side.Left)
            {
                for (var i = MinY; i <= MaxY; i++)
                {
                    yield return (MinX, i);
                }
            }
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