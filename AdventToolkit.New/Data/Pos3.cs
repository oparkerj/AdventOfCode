using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

/// <summary>
/// A 3-dimensional position.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <param name="Z"></param>
/// <typeparam name="T"></typeparam>
public readonly record struct Pos3<T>(T X, T Y, T Z) : IPos<Pos3<T>, T>
    where T : INumber<T>
{
    public static Pos3<T> Zero => default;

    public static Pos3<T> AdditiveIdentity => Zero;

    public static Pos3<T> operator +(Pos3<T> left, Pos3<T> right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Pos3<T> operator -(Pos3<T> left, Pos3<T> right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Pos3<T> operator *(Pos3<T> left, Pos3<T> right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    public static Pos3<T> operator *(Pos3<T> left, T right) => new(left.X * right, left.Y * right, left.Z * right);

    public static Pos3<T> operator /(Pos3<T> left, Pos3<T> right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    public static Pos3<T> operator /(Pos3<T> left, T right) => new(left.X / right, left.Y / right, left.Z / right);

    public static Pos3<T> operator -(Pos3<T> value) => new(-value.X, -value.Y, -value.Z);
    
    public static Pos3<T> operator --(Pos3<T> value) => new(value.X - T.One, value.Y - T.One, value.Z - T.One);

    public static Pos3<T> operator ++(Pos3<T> value) => new(value.X + T.One, value.Y + T.One, value.Z + T.One);
    
    public static Pos3<T> ParseSimple(ReadOnlySpan<char> span, char separator = ',')
    {
        var split0 = span.IndexOf(separator);
        Debug.Assert(split0 > -1, "Input contains no separator.");
        var split1 = span.LastIndexOf(separator);
        Debug.Assert(split1 > -1 && split0 != split1, "Input only contains one separator.");
        return new Pos3<T>(T.Parse(span[..split0].Trim(), null),
            T.Parse(span[(split0 + 1)..split1].Trim(), null),
            T.Parse(span[(split1 + 1)..].Trim(), null));
    }

    public static Pos3<T> Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos3<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        Debug.Assert(!s.IsEmpty, "Span is empty.");
        if (s is ['(', .. var parenInner, ')'])
        {
            s = parenInner;
        }
        else if (s is ['<', .. var bracketInner, '>'])
        {
            s = bracketInner;
        }
        Debug.Assert(!s.IsEmpty, "Inside of brackets is empty.");

        var split0 = s.IndexOfAny(',', 'x');
        Debug.Assert(split0 > -1, "Input has no separator.");
        var split1 = s.LastIndexOfAny(',', 'x');
        Debug.Assert(split1 > -1 && split0 != split1, "Input only contains one separator.");

        return new Pos3<T>(T.Parse(s[..split0].Trim(), provider),
            T.Parse(s[(split0 + 1)..split1].Trim(), provider),
            T.Parse(s[(split1 + 1)..].Trim(), provider));
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos3<T> result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos3<T> result)
    {
        if (s.IsEmpty)
        {
            result = default;
            return false;
        }

        if (s is ['(', .. var parenInner, ')'])
        {
            s = parenInner;
        }
        else if (s is ['<', .. var bracketInner, '>'])
        {
            s = bracketInner;
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
        throw new FormatException($"Unknown format for {nameof(Pos3<T>)}");

        bool ParseSplit(ReadOnlySpan<char> span, int split, int split2, out Pos3<T> result)
        {
            if (T.TryParse(span[..split].Trim(), provider, out var x) &&
                T.TryParse(span[(split + 1)..split2].Trim(), provider, out var y) &&
                T.TryParse(span[(split2 + 1)..].Trim(), provider, out var z))
            {
                result = new Pos3<T>(x, y, z);
                return true;
            }
            result = default;
            return false;
        }
    }

    public T Dist(Pos3<T> other) => T.Abs(X - other.X) + T.Abs(Y - other.Y) + T.Abs(Z - other.Z);

    public T Min() => T.Min(X, T.Min(Y, Z));

    public T Max() => T.Max(X, T.Max(Y, Z));

    public Pos3<T> Min(Pos3<T> other) => new(T.Min(X, other.X), T.Min(Y, other.Y), T.Min(Z, other.Z));

    public Pos3<T> Max(Pos3<T> other) => new(T.Max(X, other.X), T.Max(Y, other.Y), T.Max(Z, other.Z));

    public Pos3<T> Normalize() => new(X.Sign(), Y.Sign(), Z.Sign());
    
    public IEnumerable<Pos3<T>> Adjacent()
    {
        yield return this with {Z = Z + T.One};
        yield return this with {Y = Y + T.One};
        yield return this with {X = X - T.One};
        yield return this with {X = X + T.One};
        yield return this with {Y = Y - T.One};
        yield return this with {Z = Z - T.One};
    }

    public IEnumerable<Pos3<T>> Around()
    {
        // TODO
        yield break;
    }

    public override string ToString() => $"({X}, {Y}, {Z})";
}