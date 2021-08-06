using System;

namespace AdventToolkit.Utilities
{
    public readonly struct Pos : IAdd<Pos>
    {
        public readonly int X;
        public readonly int Y;

        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Pos Origin = new();

        public static implicit operator Pos((int x, int y) p) => new(p.x, p.y);

        // public static implicit operator (int x, int y)(Pos p) => (p.X, p.Y);

        public override bool Equals(object obj)
        {
            return obj is Pos (var x, var y) && x == X && y == Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
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

        public static Pos operator -(Pos p) => new(-p.X, -p.Y);

        public static Pos operator +(Pos a, Pos b) => new(a.X + b.X, a.Y + b.Y);
        public Pos Add(Pos other) => this + other;

        public static Pos operator -(Pos a, Pos b) => new(a.X - b.X, a.Y - b.Y);

        public static Pos operator *(Pos p, int s) => new(p.X * s, p.Y * s);

        public static Pos operator *(int s, Pos p) => p * s;

        public Pos Clockwise(Pos center = default) => new(Y - center.Y + center.X, center.X - X + center.Y);

        public Pos CounterClockwise(Pos center = default) => new(center.Y - Y + center.X, X - center.X + center.Y);

        public int MDist(Pos p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }

        public Pos Flip() => new(Y, X);
    }
}