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
public record Rect<T>(Interval<T> X, Interval<T> Y) : IRect<Rect<T>, T>
    where T : INumber<T>
{
    public Rect(Pos<T> a, Pos<T> b) : this(Interval<T>.Span(a.X, b.X), Interval<T>.Span(a.Y, b.Y)) { }
    
    public Rect(T width, T height) : this(new Interval<T>(width), new Interval<T>(height)) { }
    
    public static Rect<T> From(Pos<T> start, Pos<T> end)
    {
        return new Rect<T>(Interval<T>.From(start.X, end.X), Interval<T>.From(start.Y, end.Y));
    }

    public static Rect<T> Span(Pos<T> a, Pos<T> b) => new(a, b);
    
    public static Rect<T> SpanAll(IEnumerable<Pos<T>> points)
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

        return new Rect<T>(min, max);
    }

    public static Rect<T> Single(Pos<T> value)
    {
        return new Rect<T>(Interval<T>.Single(value.X), Interval<T>.Single(value.Y));
    }

    public static Rect<T> Empty => new(Interval<T>.Empty, Interval<T>.Empty);
    
    public static implicit operator Rect<T>(Pos<T> num) => new(num.X, num.Y);

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

    public Rect<TOut> As<TOut>() where TOut : INumber<TOut> => new(X.As<TOut>(), Y.As<TOut>());

    public bool Contains(Pos<T> p) => X.Contains(p.X) && Y.Contains(p.Y);
    
    public bool Contains(Rect<T> t) => X.Contains(t.X) && Y.Contains(t.Y);

    public Rect<T> Add(Pos<T> num)
    {
        return Contains(num) ? this : From(Min.Min(num), End.Max(num + Pos<T>.One));
    }

    public Rect<T> Intersect(Rect<T> other) => new(X.Intersect(other.X), Y.Intersect(other.Y));

    public Enumerator GetEnumerator() => new(X, Y);
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    IEnumerator<Pos<T>> IEnumerable<Pos<T>>.GetEnumerator() => GetEnumerator();

    public struct Enumerator(Interval<T> x, Interval<T> y) : IEnumerator<Pos<T>>
    {
        private T _currentX = x.Start - T.One;
        private T _currentY = y.Start;

        public Pos<T> Current => new(_currentX, _currentY);

        public bool MoveNext()
        {
            if (++_currentX < x.End) return true;
            _currentX = x.Start;
            return ++_currentY < y.End;
        }

        public void Reset() => throw new NotSupportedException();

        object IEnumerator.Current => Current;
        
        public void Dispose() { }
    }
}