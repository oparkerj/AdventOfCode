using System;
using System.Numerics;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Arithmetic;

namespace AdventToolkit.Common;

public readonly struct Pos3D : IAdditionOperators<Pos3D, Pos3D, Pos3D>, ISub<Pos3D>, IUnaryNegationOperators<Pos3D, Pos3D>
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public Pos3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Pos3D Origin => new();
    public static Pos3D Up => new(0, 1, 0);
    public static Pos3D Right = new(1, 0, 0);
    public static Pos3D Down = new(0, -1, 0);
    public static Pos3D Left = new(-1, 0, 0);
    public static Pos3D Forward = new(0, 0, 1);
    public static Pos3D Backward = new(0, 0, -1);

    public static implicit operator Pos3D((int x, int y, int z) p) => new(p.x, p.y, p.z);
        
    public static Pos3D Parse(string s)
    {
        if (s.StartsWith('(') && s.EndsWith(')')) s = s[1..^1];
        if (s.StartsWith('<') && s.EndsWith('>')) s = s[1..^1];
        var parts = s.Csv();
        return new Pos3D(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()), int.Parse(parts[2].Trim()));
    }

    public static Pos3D ParseHexDir(string s)
    {
        return s.ToLower() switch
        {
            "n" => new Pos3D(0, -1, 1),
            "ne" => new Pos3D(1, -1, 0),
            "se" => new Pos3D(1, 0, -1),
            "s" => new Pos3D(0, 1, -1),
            "sw" => new Pos3D(-1, 1, 0),
            "nw" => new Pos3D(-1, 0, 1),
            _ => throw new Exception("Invalid direction.")
        };
    }

    public bool Equals(Pos3D p) => X == p.X && Y == p.Y && Z == p.Z;

    public override bool Equals(object obj)
    {
        return obj is Pos3D p && Equals(p);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public void Deconstruct(out int x, out int y, out int z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public static Pos3D operator -(Pos3D p) => new(-p.X, -p.Y, -p.Z);
    public Pos3D Negate() => -this;
    public static Pos3D Negate(Pos3D v) => -v;

    public static Pos3D operator +(Pos3D a, Pos3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public Pos3D Add(Pos3D other) => this + other;
    public static Pos3D Add(Pos3D a, Pos3D b) => a + b;

    public static Pos3D operator -(Pos3D a, Pos3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public Pos3D Sub(Pos3D other) => this - other;
    public static Pos3D Sub(Pos3D a, Pos3D b) => a - b;

    public static Pos3D operator *(Pos3D p, int s) => new(p.X * s, p.Y * s, p.Z * s);

    public static Pos3D operator *(int s, Pos3D p) => p * s;

    public static Pos3D operator /(Pos3D p, int s) => new(p.X / s, p.Y / s, p.Z / s);
        
    public static Pos3D operator /(int s, Pos3D p) => new(s / p.X, s / p.Y, s / p.Z);

    public static bool operator ==(Pos3D a, Pos3D b) => a.Equals(b);
        
    public static bool operator !=(Pos3D a, Pos3D b) => !a.Equals(b);

    public Pos3D Abs() => new(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));

    public Pos To2D => new(X, Y);

    public Pos3D Normalize() => new(Math.Sign(X), Math.Sign(Y), Math.Sign(Z));

    public int MDist(Pos3D p)
    {
        return Math.Abs(X - p.X) + Math.Abs(Y - p.Y) + Math.Abs(Z - p.Z);
    }
        
    public int Magnitude()
    {
        return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    }

    public int Dot(Pos3D o) => X * o.X + Y * o.Y + Z * o.Z;

    public Pos3D XClockwise(Pos3D center = default) => new(X, Z - center.Z + center.Y, center.Y - Y + center.Z);

    public Pos3D XCounterClockwise(Pos3D center = default) => new(X, -(Z - center.Z) + center.Y, -(center.Y - Y) + center.Z);

    public Pos3D YClockwise(Pos3D center = default) => new(Z - center.Z + center.X, Y, center.X - X + center.Z);

    public Pos3D YCounterClockwise(Pos3D center = default) => new(-(Z - center.Z) + center.X, Y, -(center.X - X) + center.Z);

    public Pos3D ZClockwise(Pos3D center = default) => new(Y - center.Y + center.X, center.X - X + center.Y, Z);

    public Pos3D ZCounterClockwise(Pos3D center = default) => new(-(Y - center.Y) + center.X, -(center.X - X) + center.Y, Z);
}