using System.Collections;
using System.Numerics;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

/// <summary>
/// Rect implemented as a record.
/// Uses intervals to represent horizontal and vertical ranges.
/// </summary>
/// <param name="Horizontal"></param>
/// <param name="Vertical"></param>
/// <typeparam name="T"></typeparam>
public record Rect<T>(Interval<T> Horizontal, Interval<T> Vertical) : IRect<Rect<T>, T>
    where T : INumber<T>
{
    public Rect(Pos<T> a, Pos<T> b) : this(Interval<T>.Span(a.X, b.X), Interval<T>.Span(a.Y, b.Y)) { }
    
    public static Rect<T> From(Pos<T> start, Pos<T> end)
    {
        return new Rect<T>(Interval<T>.From(start.X, end.X), Interval<T>.From(start.Y, end.Y));
    }

    public static Rect<T> Span(Pos<T> a, Pos<T> b) => new(a, b);

    public static Rect<T> Empty => new(Interval<T>.Empty, Interval<T>.Empty);

    public T Width => Horizontal.Length;

    public T Height => Vertical.Length;

    public T MinX => Horizontal.Start;

    public T MinY => Vertical.Start;

    public T MaxX => Horizontal.Last;

    public T MaxY => Vertical.Last;

    public T EndX => Horizontal.End;

    public T EndY => Vertical.End;

    public Pos<T> Min => new(MinX, MinY);

    public Pos<T> Max => new(MaxX, MaxY);

    public Pos<T> End => new(EndX, EndY);

    public Pos<T> Size => new(Width, Height);

    public bool Contains(Pos<T> p) => Horizontal.Contains(p.X) && Vertical.Contains(p.Y);
    
    public bool Contains(Rect<T> t) => Horizontal.Contains(t.Horizontal) && Vertical.Contains(t.Vertical);

    public Rect<T> Intersect(Rect<T> other) => new(Horizontal.Intersect(other.Horizontal), Vertical.Intersect(other.Vertical));

    public Enumerator GetEnumerator() => new(Horizontal, Vertical);
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    IEnumerator<Pos<T>> IEnumerable<Pos<T>>.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<Pos<T>>
    {
        private readonly Interval<T> _x;
        private readonly Interval<T> _y;
        private T _currentX;
        private T _currentY;

        public Enumerator(Interval<T> x, Interval<T> y)
        {
            _x = x;
            _y = y;
            _currentX = x.Start - T.One;
            _currentY = y.Start;
        }

        public Pos<T> Current => new(_currentX, _currentY);

        public bool MoveNext()
        {
            if (++_currentX < _x.End) return true;
            _currentX = _x.Start;
            return ++_currentY < _y.End;
        }

        public void Reset() => throw new NotSupportedException();

        object IEnumerator.Current => Current;
        
        public void Dispose() { }
    }
}