using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

public readonly record struct Pos4<T>(T W, T X, T Y, T Z) : IPos<Pos4<T>, T>
    where T : INumber<T>
{
    public static Pos4<T> Zero => default;

    public static Pos4<T> AdditiveIdentity => Zero;

    public static Pos4<T> operator +(Pos4<T> left, Pos4<T> right) => new(left.W + right.W, left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Pos4<T> operator -(Pos4<T> left, Pos4<T> right) => new(left.W - right.W, left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Pos4<T> operator *(Pos4<T> left, Pos4<T> right) => new(left.W * right.W, left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Pos4<T> operator *(Pos4<T> left, T right) => new(left.W * right, left.X * right, left.Y * right, left.Z * right);

    public static Pos4<T> operator /(Pos4<T> left, Pos4<T> right) => new(left.W / right.W, left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Pos4<T> operator /(Pos4<T> left, T right) => new(left.W + right, left.X / right, left.Y / right, left.Z / right);

    public static Pos4<T> operator -(Pos4<T> value) => new(-value.W, -value.X, -value.Y, -value.Z);
    
    public static Pos4<T> operator --(Pos4<T> value) => new(value.X - T.One, value.X - T.One, value.Y - T.One, value.Z - T.One);

    public static Pos4<T> operator ++(Pos4<T> value) => new(value.X + T.One, value.X + T.One, value.Y + T.One, value.Z + T.One);
    
    public static Pos4<T> ParseSimple(ReadOnlySpan<char> span, char separator = ',')
    {
        var split0 = span.IndexOf(separator);
        Debug.Assert(split0 > -1, "Input contains no separator.");
        var split2 = span.LastIndexOf(separator);
        Debug.Assert(split2 > -1 && split2 != split0, "Input only contains one separator.");
        var mid = span[(split0 + 1)..split2];
        var split1 = mid.IndexOf(separator);
        Debug.Assert(split1 > -1, "Input only contains two separators.");
        return new Pos4<T>(T.Parse(span[..split0].Trim(), null),
            T.Parse(mid[..split1].Trim(), null),
            T.Parse(mid[(split1 + 1)..].Trim(), null),
            T.Parse(span[(split2 + 1)..].Trim(), null));
    }

    public static Pos4<T> Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos4<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var pos)) return pos;
        throw new FormatException($"Unknown format for {nameof(Pos4<T>)}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos4<T> result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos4<T> result)
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
        throw new FormatException($"Unknown format for {nameof(Pos4<T>)}");

        bool ParseSplit(ReadOnlySpan<char> span, int split, int split2, int split3, out Pos4<T> result)
        {
            if (T.TryParse(span[..split].Trim(), provider, out var w) &&
                T.TryParse(span[(split + 1)..split2].Trim(), provider, out var x) &&
                T.TryParse(span[(split2 + 1)..split3].Trim(), provider, out var y) &&
                T.TryParse(span[(split3 + 1)..].Trim(), provider, out var z))
            {
                result = new Pos4<T>(w, x, y, z);
                return true;
            }
            result = default;
            return false;
        }
    }
    
    public IEnumerable<Pos4<T>> Adjacent()
    {
        yield break;
    }

    public IEnumerable<Pos4<T>> Around()
    {
        yield break;
    }

    public T Dist(Pos4<T> other) => T.Abs(W - other.W) + T.Abs(X - other.X) + T.Abs(Y - other.Y) + T.Abs(Z - other.Z);

    public T Min() => T.Min(W, T.Min(X, T.Min(Y, Z)));

    public T Max() => T.Max(W, T.Max(X, T.Max(Y, Z)));

    public Pos4<T> Min(Pos4<T> other) => new(T.Min(W, other.W), T.Min(X, other.X), T.Min(Y, other.Y), T.Min(Z, other.Z));

    public Pos4<T> Max(Pos4<T> other) => new(T.Max(W, other.W), T.Max(X, other.X), T.Max(Y, other.Y), T.Max(Z, other.Z));

    public Pos4<T> Normalize() => new(W.Sign(), X.Sign(), Y.Sign(), Z.Sign());

    public override string ToString() => $"({W}, {X}, {Y}, {Z})";
}