using System;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Arithmetic;

namespace AdventToolkit.Utilities
{
    public readonly struct Pos : IAdd<Pos>, ISub<Pos>, INegate<Pos>
    {
        public readonly int X;
        public readonly int Y;

        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Pos Origin => new();
        public static Pos Up => new(0, 1);
        public static Pos Right => new(1, 0);
        public static Pos Down => new(0, -1);
        public static Pos Left => new(-1, 0);
        public static Pos ConsoleUp => Down;
        public static Pos ConsoleDown => Up;

        public static implicit operator Pos((int x, int y) p) => new(p.x, p.y);

        // public static implicit operator (int x, int y)(Pos p) => (p.X, p.Y);

        public bool Equals(Pos p) => X == p.X && Y == p.Y;

        public override bool Equals(object obj)
        {
            return obj is Pos p && Equals(p);
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
        public Pos Negate() => -this;

        public static Pos operator +(Pos a, Pos b) => new(a.X + b.X, a.Y + b.Y);
        public Pos Add(Pos other) => this + other;

        public static Pos operator -(Pos a, Pos b) => new(a.X - b.X, a.Y - b.Y);
        public Pos Sub(Pos other) => this - other;

        public static Pos operator *(Pos p, int s) => new(p.X * s, p.Y * s);

        public static Pos operator *(int s, Pos p) => p * s;

        public static Pos operator /(Pos p, int s) => new(p.X / s, p.Y / s);
        
        public static Pos operator /(int s, Pos p) => new(s / p.X, s / p.Y);

        public static bool operator ==(Pos a, Pos b) => a.Equals(b);
        
        public static bool operator !=(Pos a, Pos b) => !a.Equals(b);

        public Pos Normalize() => new(Math.Sign(X), Math.Sign(Y));
        
        public Pos Towards(Pos other)
        {
            var delta = other - this;
            if (delta.X == 0 || delta.Y == 0) return delta.Normalize();
            return delta / delta.X.Gcd(delta.Y);
        }

        public Pos Clockwise(Pos center = default) => new(Y - center.Y + center.X, center.X - X + center.Y);

        public Pos CounterClockwise(Pos center = default) => new(center.Y - Y + center.X, X - center.X + center.Y);

        public int MDist(Pos p)
        {
            return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
        }

        public Pos Flip() => new(Y, X);

        public int Cross(Pos other)
        {
            return X * other.Y - Y * other.X;
        }
    }
}