using System;
using System.Numerics;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Arithmetic;

namespace AdventToolkit.Common;

public readonly struct Pos4D : IAdditionOperators<Pos4D, Pos4D, Pos4D>, ISub<Pos4D>, IUnaryNegationOperators<Pos4D, Pos4D>
{
    public readonly int W;
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public Pos4D(int w, int x, int y, int z)
    {
        W = w;
        X = x;
        Y = y;
        Z = z;
    }

    public static Pos4D Zero => new();

    public static implicit operator Pos4D((int w, int x, int y, int z) p) => new(p.w, p.x, p.y, p.z);
        
    public static Pos4D Parse(string s)
    {
        if (s.StartsWith('(') && s.EndsWith(')')) s = s[1..^1];
        if (s.StartsWith('<') && s.EndsWith('>')) s = s[1..^1];
        var parts = s.Csv();
        return new Pos4D(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()), int.Parse(parts[2].Trim()), int.Parse(parts[3].Trim()));
    }

    public bool Equals(Pos4D p) => W == p.W && X == p.X && Y == p.Y && Z == p.Z;

    public override bool Equals(object obj)
    {
        return obj is Pos4D p && Equals(p);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(W.GetHashCode(), X.GetHashCode(), Y.GetHashCode(), Z.GetHashCode());
    }

    public override string ToString()
    {
        return $"({W}, {X}, {Y}, {Z})";
    }

    public void Deconstruct(out int w, out int x, out int y, out int z)
    {
        w = W;
        x = X;
        y = Y;
        z = Z;
    }

    public static Pos4D operator -(Pos4D p) => new(-p.W, -p.X, -p.Y, -p.Z);
    public Pos4D Negate() => -this;

    public static Pos4D operator +(Pos4D a, Pos4D b) => new(a.W + b.W, a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public Pos4D Add(Pos4D other) => this + other;

    public static Pos4D operator -(Pos4D a, Pos4D b) => new(a.W - b.W, a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public Pos4D Sub(Pos4D other) => this - other;

    public static Pos4D operator *(Pos4D p, int s) => new(p.W * s, p.X * s, p.Y * s, p.Z * s);

    public static Pos4D operator *(int s, Pos4D p) => p * s;

    public static Pos4D operator /(Pos4D p, int s) => new(p.W / s, p.X / s, p.Y / s, p.Z / s);
        
    public static Pos4D operator /(int s, Pos4D p) => new(s / p.W, s / p.X, s / p.Y, s / p.Z);

    public static bool operator ==(Pos4D a, Pos4D b) => a.Equals(b);
        
    public static bool operator !=(Pos4D a, Pos4D b) => !a.Equals(b);

    public Pos4D Normalize() => new(Math.Sign(W), Math.Sign(X), Math.Sign(Y), Math.Sign(Z));

    public int MDist(in Pos4D p)
    {
        return Math.Abs(W - p.W) + Math.Abs(X - p.X) + Math.Abs(Y - p.Y) + Math.Abs(Z - p.Z);
    }
        
    public int Magnitude()
    {
        return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z) + Math.Abs(W);
    }
}