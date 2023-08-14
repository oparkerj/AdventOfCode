using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

public readonly record struct Pos3(int X, int Y, int Z) : IPos<Pos3, int>
{
    public static Pos3 Zero => default;

    public static Pos3 AdditiveIdentity => Zero;

    public static Pos3 operator +(Pos3 left, Pos3 right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Pos3 operator -(Pos3 left, Pos3 right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Pos3 operator *(Pos3 left, Pos3 right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Pos3 operator *(Pos3 left, int right) => new(left.X * right, left.Y * right, left.Z * right);

    public static Pos3 operator /(Pos3 left, Pos3 right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Pos3 operator /(Pos3 left, int right) => new(left.X / right, left.Y / right, left.Z / right);

    public static Pos3 operator -(Pos3 value) => new(-value.X, -value.Y, -value.Z);

    public static Pos3 Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos3 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var pos)) return pos;
        throw new FormatException($"Unknown format for {nameof(Pos3)}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos3 result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos3 result)
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
            var comma2 = s.LastIndexOf(',');
            return ParseSplit(s, comma, comma2, out result);
        }
        if (s.IndexOf('x') is var cross and > -1)
        {
            var cross2 = s.LastIndexOf('x');
            return ParseSplit(s, cross, cross2, out result);
        }
        throw new FormatException($"Unknown format for {nameof(Pos3)}");

        bool ParseSplit(ReadOnlySpan<char> span, int split, int split2, out Pos3 result)
        {
            if (int.TryParse(span[..split].Trim(), provider, out var x) &&
                int.TryParse(span[(split + 1)..split2].Trim(), provider, out var y) &&
                int.TryParse(span[(split2 + 1)..].Trim(), provider, out var z))
            {
                result = new Pos3(x, y, z);
                return true;
            }
            result = default;
            return false;
        }
    }

    public int Dist(Pos3 other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);

    public int Min() => Math.Min(X, Math.Min(Y, Z));

    public int Max() => Math.Max(X, Math.Max(Y, Z));

    public Pos3 Min(Pos3 other) => new(Math.Min(X, other.X), Math.Min(Y, other.Y), Math.Min(Z, other.Z));

    public Pos3 Max(Pos3 other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y), Math.Max(Z, other.Z));

    public Pos3 Normalize() => new(Math.Sign(X), Math.Sign(Y), Math.Sign(Z));
    
    public IEnumerable<Pos3> Adjacent()
    {
        yield return this with {Z = Z + 1};
        yield return this with {Y = Y + 1};
        yield return this with {X = X - 1};
        yield return this with {X = X + 1};
        yield return this with {Y = Y - 1};
        yield return this with {Z = Z - 1};
    }

    public IEnumerable<Pos3> Around()
    {
        yield break;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}