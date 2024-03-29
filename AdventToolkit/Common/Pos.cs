using System;
using System.Collections.Generic;
using System.Numerics;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventToolkit.Common;

public readonly struct Pos : IAdditionOperators<Pos, Pos, Pos>,
        ISubtractionOperators<Pos, Pos, Pos>,
        IUnaryNegationOperators<Pos, Pos>,
        IAdditiveIdentity<Pos, Pos>
{
    public readonly int X;
    public readonly int Y;

    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Pos Origin => new();
    public static Pos Zero => new();
    public static Pos Up => new(0, 1);
    public static Pos Right => new(1, 0);
    public static Pos Down => new(0, -1);
    public static Pos Left => new(-1, 0);
    public static Pos ConsoleUp => Down;
    public static Pos ConsoleDown => Up;

    private static IComparer<Pos> _readingOrder;
    public static IComparer<Pos> ReadingOrder => _readingOrder ??= Comparing<Pos>.ByReverse(pos => pos.Y).ThenBy(pos => pos.X);

    public static implicit operator Pos((int x, int y) p) => new(p.x, p.y);

    public static IEnumerable<Pos> Directions => GetDirections();

    public static IEnumerable<Pos> GetDirections(bool diagonal = false)
    {
        return diagonal ? Origin.Around() : Origin.Adjacent();
    }

    public static Pos Parse(string s)
    {
        if (s.StartsWith('(') && s.EndsWith(')')) s = s[1..^1];
        if (s.StartsWith('<') && s.EndsWith('>')) s = s[1..^1];
        if (s.Contains(','))
        {
            var parts = s.Csv();
            return new Pos(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
        }
        if (s.Contains('x'))
        {
            var (left, right) = s.SingleSplit('x');
            return new Pos(left.Trim().AsInt(), right.Trim().AsInt());
        }
        throw new FormatException("Unknown format.");
    }

    public static Pos Index(int i)
    {
        return i switch
        {
            0 => new Pos(1, 0),
            1 => new Pos(0, 1),
            _ => throw new Exception("Invalid index")
        };
    }

    public static IEnumerable<char> RelativeDirections => "UDRL";

    public static Pos ParseRelative(string s)
    {
        var dir = RelativeDirection(s[0]);
        var length = s[1..].AsInt();
        return dir * length;
    }

    public static Pos RelativeDirection(char c)
    {
        return char.ToLower(c) switch
        {
            'u' or '^' => Up,
            'r' or '>' => Right,
            'l' or '<' => Left,
            'd' or 'v' => Down,
            'f' => Right,
            'b' => Left,
            _ => throw new Exception("Invalid direction."),
        };
    }
    
    public int this[int i] =>
        i switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
        };

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

    public static Pos AdditiveIdentity => new();

    public static Pos operator -(Pos p) => new(-p.X, -p.Y);
    public Pos Negate() => -this;

    public static Pos operator +(Pos a, Pos b) => new(a.X + b.X, a.Y + b.Y);
    public Pos Add(Pos other) => this + other;
    public static Pos Add(Pos a, Pos b) => a + b;

    public static Pos operator -(Pos a, Pos b) => new(a.X - b.X, a.Y - b.Y);
    public Pos Sub(Pos other) => this - other;

    public static Pos operator *(Pos p, int s) => new(p.X * s, p.Y * s);

    public static Pos operator *(int s, Pos p) => p * s;

    // Component-wise multiplication
    public static Pos operator *(Pos a, Pos b) => new(a.X * b.X, a.Y * b.Y);

    public static Pos operator /(Pos p, int s) => new(p.X / s, p.Y / s);
        
    public static Pos operator /(int s, Pos p) => new(s / p.X, s / p.Y);

    // Component-wise division
    public static Pos operator /(Pos a, Pos b) => new(a.X / b.X, a.Y / b.Y);

    public static bool operator ==(Pos a, Pos b) => a.Equals(b);
        
    public static bool operator !=(Pos a, Pos b) => !a.Equals(b);

    public bool NonNegative => X >= 0 && Y >= 0;

    public Pos3D To3D(int z) => new(X, Y, z);

    public Pos Invert() => new(X, -Y);

    public Pos Normalize() => new(Math.Sign(X), Math.Sign(Y));
        
    public Pos Towards(Pos other)
    {
        var delta = other - this;
        if (delta.X == 0 || delta.Y == 0) return delta.Normalize();
        return delta / delta.X.Gcd(delta.Y);
    }

    public Pos Extend(int length) => this + Normalize() * length;

    public Pos Clockwise(Pos center = default) => new(Y - center.Y + center.X, center.X - X + center.Y);

    public Pos CounterClockwise(Pos center = default) => new(center.Y - Y + center.X, X - center.X + center.Y);

    public int GetDirection()
    {
        var d = Normalize();
        return d switch
        {
            (0, 1) => 0,
            (1, 0) => 1,
            (0, -1) => 2,
            (-1, 0) => 3,
            _ => -1,
        };
    }

    public Pos Turn(int dir)
    {
        if (dir < 0) return CounterClockwise();
        if (dir > 0) return Clockwise();
        return this;
    }

    public int MDist(Pos p)
    {
        return Math.Abs(X - p.X) + Math.Abs(Y - p.Y);
    }

    public long MDistSquared(Pos p)
    {
        var (dx, dy) = this - p;
        return (long) dx * dx + (long) dy * dy;
    }

    public int Magnitude()
    {
        return Math.Abs(X) + Math.Abs(Y);
    }

    public double RealDist(Pos p) => Math.Sqrt(RealDistSquared(p));

    public double RealDistSquared(Pos p)
    {
        var (dx, dy) = p - this;
        return (double) dx * dx + (double) dy * dy;
    }

    public Pos Flip() => new(Y, X);

    public int Dot(Pos other) => X * other.X + Y * other.Y;

    public int Cross(Pos other)
    {
        return X * other.Y - Y * other.X;
    }

    public Pos Min(Pos other) => new(Math.Min(X, other.X), Math.Min(Y, other.Y));

    public int Min() => Math.Min(X, Y);
        
    public Pos Max(Pos other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));

    public int Max() => Math.Max(X, Y);
}