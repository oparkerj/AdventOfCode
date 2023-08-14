using System.Numerics;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

public readonly record struct Pos2<T>(T X, T Y) : IPos<Pos2<T>, T>
    where T : INumber<T>
{
    public static Pos2<T> Zero => default;

    public static Pos2<T> AdditiveIdentity => Zero;

    public static Pos2<T> operator +(Pos2<T> left, Pos2<T> right) => new(left.X + right.X, left.Y + right.Y);

    public static Pos2<T> operator -(Pos2<T> left, Pos2<T> right) => new(left.X - right.X, left.Y - right.Y);

    public static Pos2<T> operator *(Pos2<T> left, Pos2<T> right) => new(left.X * right.X, left.Y * right.Y);

    public static Pos2<T> operator *(Pos2<T> left, T right) => new(left.X * right, left.Y * right);

    public static Pos2<T> operator /(Pos2<T> left, Pos2<T> right) => new(left.X / right.X, left.Y / right.Y);

    public static Pos2<T> operator /(Pos2<T> left, T right) => new(left.X / right, left.Y / right);

    public static Pos2<T> operator -(Pos2<T> value) => new(-value.X, -value.Y);

    public static Pos2<T> Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos2<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var pos)) return pos;
        throw new FormatException($"Unknown format for {nameof(Pos2<T>)}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos2<T> result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos2<T> result)
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
            return ParseSplit(s, comma, out result);
        }
        if (s.IndexOf('x') is var cross and > -1)
        {
            return ParseSplit(s, cross, out result);
        }
        throw new FormatException($"Unknown format for {nameof(Pos2<T>)}");

        bool ParseSplit(ReadOnlySpan<char> span, int split, out Pos2<T> result)
        {
            if (T.TryParse(span[..split].Trim(), provider, out var x) &&
                T.TryParse(span[(split + 1)..].Trim(), provider, out var y))
            {
                result = new Pos2<T>(x, y);
                return true;
            }
            result = default;
            return false;
        }
    }

    public T Dist(Pos2<T> other) => T.Abs(X - other.X) + T.Abs(Y - other.Y);

    public T Min() => T.Min(X, Y);

    public T Max() => T.Max(X, Y);

    public Pos2<T> Min(Pos2<T> other) => new(T.Min(X, other.X), T.Min(Y, other.Y));

    public Pos2<T> Max(Pos2<T> other) => new(T.Max(X, other.X), T.Max(Y, other.Y));

    private static T Sign(T value) => value != T.Zero ? (T.IsNegative(value) ? -T.One : T.One) : T.Zero;

    public Pos2<T> Normalize() => new(Sign(X), Sign(Y));

    public IEnumerable<Pos2<T>> Adjacent()
    {
        yield return this with {Y = Y + T.One};
        yield return this with {X = X - T.One};
        yield return this with {X = X + T.One};
        yield return this with {Y = Y - T.One};
    }

    public IEnumerable<Pos2<T>> Around()
    {
        yield return new Pos2<T>(X - T.One, Y + T.One);
        yield return this with {Y = Y + T.One};
        yield return new Pos2<T>(X + T.One, Y + T.One);
        yield return this with {X = X - T.One};
        yield return this with {X = X + T.One};
        yield return new Pos2<T>(X - T.One, Y - T.One);
        yield return this with {Y = Y - T.One};
        yield return new Pos2<T>(X + T.One, Y - T.One);
    }

    public override string ToString() => $"({X}, {Y})";
}