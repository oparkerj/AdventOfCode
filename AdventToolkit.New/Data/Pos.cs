using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

public readonly record struct Pos(int X, int Y) : IPos<Pos, int>
{
    public static Pos Zero => default;

    public static Pos AdditiveIdentity => Zero;

    public static Pos operator +(Pos left, Pos right) => new(left.X + right.X, left.Y + right.Y);

    public static Pos operator -(Pos left, Pos right) => new(left.X - right.X, left.Y - right.Y);

    public static Pos operator *(Pos left, Pos right) => new(left.X * right.X, left.Y * right.Y);

    public static Pos operator *(Pos left, int right) => new(left.X * right, left.Y * right);

    public static Pos operator /(Pos left, Pos right) => new(left.X / right.X, left.Y / right.Y);

    public static Pos operator /(Pos left, int right) => new(left.X / right, left.Y / right);

    public static Pos operator -(Pos value) => new(-value.X, -value.Y);

    public static Pos Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var pos)) return pos;
        throw new FormatException($"Unknown format for {nameof(Pos)}");
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos result)
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

        if (s.IndexOfAny(',', 'x') is var split and > -1 &&
            int.TryParse(s[..split].Trim(), provider, out var x) &&
            int.TryParse(s[(split + 1)..].Trim(), provider, out var y))
        {
            result = new Pos(x, y);
            return true;
        }
        throw new FormatException($"Unknown format for {nameof(Pos)}");
    }

    public int Dist(Pos other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);

    public int Min() => Math.Min(X, Y);

    public int Max() => Math.Max(X, Y);

    public Pos Min(Pos other) => new(Math.Min(X, other.X), Math.Min(Y, other.Y));

    public Pos Max(Pos other) => new(Math.Max(X, other.X), Math.Max(Y, other.Y));

    public Pos Normalize() => new(Math.Sign(X), Math.Sign(Y));
    
    public IEnumerable<Pos> Adjacent()
    {
        yield return this with {Y = Y + 1};
        yield return this with {X = X - 1};
        yield return this with {X = X + 1};
        yield return this with {Y = Y - 1};
    }

    public IEnumerable<Pos> Around()
    {
        yield return new Pos(X - 1, Y + 1);
        yield return this with {Y = Y + 1};
        yield return new Pos(X + 1, Y + 1);
        yield return this with {X = X - 1};
        yield return this with {X = X + 1};
        yield return new Pos(X - 1, Y - 1);
        yield return this with {Y = Y - 1};
        yield return new Pos(X + 1, Y - 1);
    }

    public override string ToString() => $"({X}, {Y})";
}