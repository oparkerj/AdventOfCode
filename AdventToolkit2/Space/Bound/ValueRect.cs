using System.Collections;
using System.Numerics;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Bound;

/// <summary>
/// Rect implemented as a record.
/// Uses intervals to represent horizontal and vertical ranges.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <typeparam name="T"></typeparam>
public readonly record struct ValueRect<T>(Interval<T> X, Interval<T> Y) : IRect<ValueRect<T>, T>
    where T : INumber<T>
{
    public ValueRect(Pos<T> a, Pos<T> b) : this(Interval<T>.Span(a.X, b.X), Interval<T>.Span(a.Y, b.Y)) { }
    
    public ValueRect(T width, T height) : this(new Interval<T>(width), new Interval<T>(height)) { }
    
    public static ValueRect<T> From(Pos<T> start, Pos<T> end)
    {
        return new ValueRect<T>(Interval<T>.From(start.X, end.X), Interval<T>.From(start.Y, end.Y));
    }

    public static ValueRect<T> Span(Pos<T> a, Pos<T> b) => new(a, b);
    
    public static ValueRect<T> SpanAll(IEnumerable<Pos<T>> points)
    {
        using var enumerator = points.GetEnumerator();
        if (!enumerator.MoveNext()) return Empty;

        var min = enumerator.Current;
        var max = enumerator.Current;

        while (enumerator.MoveNext())
        {
            min = min.Min(enumerator.Current);
            max = max.Max(enumerator.Current);
        }

        return new ValueRect<T>(min, max);
    }

    public static ValueRect<T> Single(Pos<T> value)
    {
        return new ValueRect<T>(Interval<T>.Single(value.X), Interval<T>.Single(value.Y));
    }

    public static ValueRect<T> Empty => new(Interval<T>.Empty, Interval<T>.Empty);
    
    public static implicit operator ValueRect<T>(Pos<T> num) => new(num.X, num.Y);

    public T Width => X.Length;

    public T Height => Y.Length;

    public T MinX => X.Start;

    public T MinY => Y.Start;

    public T MaxX => X.Last;

    public T MaxY => Y.Last;

    public T EndX => X.End;

    public T EndY => Y.End;

    public Pos<T> Min => new(MinX, MinY);

    public Pos<T> Max => new(MaxX, MaxY);

    public Pos<T> End => new(EndX, EndY);

    public Pos<T> Size => new(Width, Height);

    /// <summary>
    /// Convert this rect to a different number type
    /// </summary>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    public ValueRect<TNew> As<TNew>()
        where TNew : INumber<TNew> =>
        new(X.As<TNew>(), Y.As<TNew>());

    public bool Contains(Pos<T> p) => X.Contains(p.X) && Y.Contains(p.Y);
    
    public bool Contains(ValueRect<T> t) => X.Contains(t.X) && Y.Contains(t.Y);

    public ValueRect<T> Add(Pos<T> num)
    {
        return Contains(num) ? this : From(Min.Min(num), End.Max(num + Pos<T>.One));
    }

    public ValueRect<T> Intersect(ValueRect<T> other) => new(X.Intersect(other.X), Y.Intersect(other.Y));

    public readonly Rect<T>.Enumerator GetEnumerator() => new(X, Y);
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    IEnumerator<Pos<T>> IEnumerable<Pos<T>>.GetEnumerator() => GetEnumerator();
}