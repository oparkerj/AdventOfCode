using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Calc;
using AdventToolkit.New.Parsing;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space;

/// <summary>
/// A 2-dimensional position.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <typeparam name="T"></typeparam>
public readonly record struct Pos<T>(T X, T Y) : IPos<Pos<T>, T>
    where T : INumber<T>
{
    public static Pos<T> Zero => default;

    public static Pos<T> AdditiveIdentity => default;
    
    public static Pos<T> Up => new(T.Zero, T.One);
    
    public static Pos<T> Down => new(T.Zero, -T.One);
    
    public static Pos<T> Right => new(T.One, T.Zero);
    
    public static Pos<T> Left => new(-T.One, T.Zero);

    public static Pos<T> operator +(Pos<T> left, Pos<T> right) => new(left.X + right.X, left.Y + right.Y);

    public static Pos<T> operator -(Pos<T> left, Pos<T> right) => new(left.X - right.X, left.Y - right.Y);

    public static Pos<T> operator *(Pos<T> left, Pos<T> right) => new(left.X * right.X, left.Y * right.Y);

    public static Pos<T> operator *(Pos<T> left, T right) => new(left.X * right, left.Y * right);

    public static Pos<T> operator /(Pos<T> left, Pos<T> right) => new(left.X / right.X, left.Y / right.Y);

    public static Pos<T> operator /(Pos<T> left, T right) => new(left.X / right, left.Y / right);

    public static Pos<T> operator -(Pos<T> value) => new(-value.X, -value.Y);
    
    public static Pos<T> operator --(Pos<T> value) => new(value.X - T.One, value.Y - T.One);

    public static Pos<T> operator ++(Pos<T> value) => new(value.X + T.One, value.Y + T.One);

    public static Pos<T> ParseSimple(ReadOnlySpan<char> span, char separator = ',')
    {
        var index = span.IndexOf(separator);
        Debug.Assert(index > -1, "Input contains no separator.");
        return new Pos<T>(T.Parse(span[..index].Trim(), null), T.Parse(span[(index + 1)..].Trim(), null));
    }

    public static Pos<T> Parse(string s, IFormatProvider? provider) => Parse(s.AsSpan(), provider);

    public static Pos<T> Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
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

        var comma = s.IndexOfAny(',', 'x');
        Debug.Assert(comma > -1, "Input has no separator.");
        
        return new Pos<T>(T.Parse(s[..comma].Trim(), provider),
            T.Parse(s[(comma + 1)..].Trim(), provider));
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Pos<T> result)
    {
        if (s is not null) return TryParse(s.AsSpan(), provider, out result);
        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Pos<T> result)
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

        if (s.IndexOfAny(',', 'x') is var split and > -1 &&
            T.TryParse(s[..split].Trim(), provider, out var x) &&
            T.TryParse(s[(split + 1)..].Trim(), provider, out var y))
        {
            result = new Pos<T>(x, y);
            return true;
        }

        result = default;
        return false;
    }

    public T Dist(Pos<T> other) => T.Abs(X - other.X) + T.Abs(Y - other.Y);

    public T Min() => T.Min(X, Y);

    public T Max() => T.Max(X, Y);

    public Pos<T> Min(Pos<T> other) => new(T.Min(X, other.X), T.Min(Y, other.Y));

    public Pos<T> Max(Pos<T> other) => new(T.Max(X, other.X), T.Max(Y, other.Y));

    public Pos<T> Normalize() => new(X.Sign(), Y.Sign());
    
    public static IEnumerable<Pos<T>> GetNeighbors(Pos<T> pos) => pos.Adjacent();

    public IEnumerable<Pos<T>> Adjacent()
    {
        yield return this with {Y = Y + T.One};
        yield return this with {X = X - T.One};
        yield return this with {X = X + T.One};
        yield return this with {Y = Y - T.One};
    }

    public IEnumerable<Pos<T>> Around()
    {
        yield return new Pos<T>(X - T.One, Y + T.One);
        yield return this with {Y = Y + T.One};
        yield return new Pos<T>(X + T.One, Y + T.One);
        yield return this with {X = X - T.One};
        yield return this with {X = X + T.One};
        yield return new Pos<T>(X - T.One, Y - T.One);
        yield return this with {Y = Y - T.One};
        yield return new Pos<T>(X + T.One, Y - T.One);
    }

    public override string ToString() => $"({X}, {Y})";

    public bool Match(Type type) => type.Generic() == typeof(Pos<>);

    public bool TryConstruct(Type type, IParseContext context, TypeSpan types, out IParser constructor)
    {
        var numType = type.GetSingleTypeArgument();
        if (types.TryAdaptTuple(Types.CreateTupleType(numType, 2), context, out var convert))
        {
            var posConstructor = type.MakeNestedType(nameof(Constructor)).NewParser();
            constructor = ParseAdapt.MaybeJoin(convert, posConstructor);
            return true;
        }

        constructor = default!;
        return false;
    }

    public bool TryUnpack(Type type, IParseContext context, int amount, out IParser unpack)
    {
        if (amount != 2)
        {
            unpack = default!;
            return false;
        }

        unpack = type.MakeNestedType(nameof(Unpack)).NewParser();
        return true;
    }

    /// <summary>
    /// Construct a Pos from a pair.
    /// </summary>
    public class Constructor : IParser<(T, T), Pos<T>>
    {
        public Pos<T> Parse((T, T) input) => new(input.Item1, input.Item2);
    }

    /// <summary>
    /// Unpack a Pos into a pair.
    /// </summary>
    public class Unpack : IParser<Pos<T>, (T, T)>
    {
        public (T, T) Parse(Pos<T> input) => (input.X, input.Y);
    }
}