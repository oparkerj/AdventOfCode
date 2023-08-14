using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

public readonly record struct Pos4(int W, int X, int Y, int Z) : IPos<Pos4, int>
{
    public static Pos4 Zero => default;

    public static Pos4 AdditiveIdentity => Zero;

    public static Pos4 operator +(Pos4 left, Pos4 right) => new(left.W + right.W, left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Pos4 operator -(Pos4 left, Pos4 right) => new(left.W - right.W, left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Pos4 operator *(Pos4 left, Pos4 right) => new(left.W * right.W, left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Pos4 operator *(Pos4 left, int right) => new(left.W * right, left.X * right, left.Y * right, left.Z * right);

    public static Pos4 operator /(Pos4 left, Pos4 right) => new(left.W / right.W, left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Pos4 operator /(Pos4 left, int right) => new(left.W + right, left.X / right, left.Y / right, left.Z / right);

    public static Pos4 operator -(Pos4 value) => new(-value.W, -value.X, -value.Y, -value.Z);

    public static Pos4 Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos4 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var pos)) return pos;
        throw new FormatException($"Unknown format for {nameof(Pos4)}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos4 result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos4 result)
    {
        if (s.IsEmpty)
        {
            result = default;
            return false;
        }

        if (s[0] == '(' && s[^1] == ')')
        {
            s = s[1..^1];
        }
        else if (s[0] == '<' && s[^1] == '>')
        {
            s = s[1..^1];
        }

        if (s.IndexOf(',') is var comma and > -1)
        {
            var comma2 = s[(comma + 1)..].IndexOf(',') + comma + 1;
            var comma3 = s.LastIndexOf(',');
            return ParseSplit(s, comma, comma2, comma3, out result);
        }
        if (s.IndexOf('x') is var cross and > -1)
        {
            var cross2 = s[(cross + 1)..].IndexOf('x') + cross + 1;
            var cross3 = s.LastIndexOf('x');
            return ParseSplit(s, cross, cross2, cross3, out result);
        }
        throw new FormatException($"Unknown format for {nameof(Pos4)}");

        bool ParseSplit(ReadOnlySpan<char> span, int split, int split2, int split3, out Pos4 result)
        {
            if (int.TryParse(span[..split].Trim(), provider, out var w) &&
                int.TryParse(span[(split + 1)..split2].Trim(), provider, out var x) &&
                int.TryParse(span[(split2 + 1)..split3].Trim(), provider, out var y) &&
                int.TryParse(span[(split3 + 1)..].Trim(), provider, out var z))
            {
                result = new Pos4(w, x, y, z);
                return true;
            }
            result = default;
            return false;
        }
    }
    
    public IEnumerable<Pos4> Adjacent()
    {
        yield break;
    }

    public IEnumerable<Pos4> Around()
    {
        yield break;
    }

    public int Dist(Pos4 other) => Math.Abs(W - other.W) + Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

    public int Min() => Math.Min(W, Math.Min(X, Math.Min(Y, Z)));

    public int Max() => Math.Max(W, Math.Max(X, Math.Max(Y, Z)));

    public Pos4 Min(Pos4 other) => new(Math.Min(W, other.W), Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));

    public Pos4 Max(Pos4 other) => new(Math.Max(W, other.W), Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));

    public Pos4 Normalize() => new(Math.Sign(W), Math.Sign(X), Math.Sign(Y), Math.Sign(Z));

    public override string ToString() => $"({W}, {X}, {Y}, {Z})";
}