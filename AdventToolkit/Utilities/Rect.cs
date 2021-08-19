using System;
using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class Rect
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }

        public bool Initialized;

        public Rect() { }

        public Rect(int minX, int minY, int maxX, int maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }

        public Rect(Pos a, Pos b)
        {
            MinX = Math.Min(a.X, b.X);
            MinY = Math.Min(a.Y, b.Y);
            MaxX = Math.Max(a.X, b.X);
            MaxY = Math.Max(a.Y, b.Y);
        }

        public Rect(int maxX, int maxY)
        {
            MaxX = maxX;
            MaxY = maxY;
        }

        public Rect(Rect other) : this(other.MinX, other.MinY, other.MaxX, other.MaxY) { }

        public static Rect Size(int width, int height)
        {
            return new Rect(width - 1, height - 1);
        }

        public void Deconstruct(out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = MinX;
            minY = MinY;
            maxX = MaxX;
            maxY = MaxY;
        }

        public void Deconstruct(out Pos min, out Pos max)
        {
            min = Min;
            max = Max;
        }
        
        public int Width => MaxX - MinX + 1;

        public int Height => MaxY - MinY + 1;

        public double MidX => (MinX + MaxX) / 2d;

        public double MidY => (MinY + MaxY) / 2d;

        public Pos Min => new(MinX, MinY);

        public Pos Max => new(MaxX, MaxY);

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

        public IEnumerable<Pos> Corners()
        {
            yield return new Pos(MinX, MinY);
            yield return new Pos(MaxX, MinY);
            yield return new Pos(MaxX, MaxY);
            yield return new Pos(MinX, MaxY);
        }

        public IEnumerable<Pos> Positions()
        {
            for (var y = MinY; y <= MaxY; y++)
            {
                for (var x = MinX; x <= MaxX; x++)
                {
                    yield return new Pos(x, y);
                }
            }
        }

        public IEnumerable<Pos> GetSidePositions(Side side)
        {
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