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

        public Rect((int x, int y) a, (int x, int y) b)
        {
            MinX = Math.Min(a.x, b.x);
            MinY = Math.Min(a.y, b.y);
            MaxX = Math.Max(a.x, b.x);
            MaxY = Math.Max(a.y, b.y);
        }

        public Rect(int maxX, int maxY)
        {
            MaxX = maxX;
            MaxY = maxY;
        }

        public Rect(Rect other) : this(other.MinX, other.MinY, other.MaxX, other.MaxY) { }

        public void Deconstruct(out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = MinX;
            minY = MinY;
            maxX = MaxX;
            maxY = MaxY;
        }
        
        public int Width => MaxX - MinX + 1;

        public int Height => MaxY - MinY + 1;

        public double MidX => (MinX + MaxX) / 2d;

        public double MidY => (MinY + MaxY) / 2d;

        public (double x, double y) Mid => (MidX, MidY);

        public Rect Fit((int x, int y) p)
        {
            var set = !Initialized;
            if (p.x < MinX || set) MinX = p.x;
            if (p.x > MaxX || set) MaxX = p.x;
            if (p.y < MinY || set) MinY = p.y;
            if (p.y > MaxY || set) MaxY = p.y;
            Initialized = true;
            return this;
        }

        public IEnumerable<(int x, int y)> Corners()
        {
            yield return (MinX, MinY);
            yield return (MaxX, MinY);
            yield return (MaxX, MaxY);
            yield return (MinX, MaxY);
        }
        
    }
}