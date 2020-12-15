using System;

namespace AdventToolkit.Data
{
    public readonly struct Pos
    {
        public readonly int X;
        public readonly int Y;

        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Pos((int x, int y) p)
        {
            return new Pos(p.x, p.y);
        }

        public static implicit operator (int x, int y)(Pos p)
        {
            return (p.X, p.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public static Pos operator -(Pos p)
        {
            return (-p.X, -p.Y);
        }

        public static Pos operator +(Pos a, Pos b)
        {
            return (a.X + b.X, a.Y + b.Y);
        }

        public static Pos operator -(Pos a, Pos b)
        {
            return (a.X - b.X, a.Y - b.Y);
        }

        public static Pos operator *(Pos p, int s)
        {
            return (p.X * s, p.Y * s);
        }

        public static Pos operator *(int s, Pos p)
        {
            return p * s;
        }

        public Pos Clockwise(Pos center = default)
        {
            return (Y - center.Y + center.X, center.X - X + center.Y);
        }

        public Pos CounterClockwise(Pos center = default)
        {
            return (center.Y - Y + center.X, X - center.X + center.Y);
        }

        public int MDist(Pos p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }
    }
}