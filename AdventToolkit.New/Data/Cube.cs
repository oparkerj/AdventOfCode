using System.Collections;
using System.Numerics;
using AdventToolkit.New.Interface;

namespace AdventToolkit.New.Data;

/// <summary>
/// Cube implemented as a record.
/// Uses intervals to represent each dimension's span.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <param name="Z"></param>
/// <typeparam name="T"></typeparam>
public record Cube<T>(Interval<T> X, Interval<T> Y, Interval<T> Z) : ICube<Cube<T>, T>
    where T : INumber<T>
{
    public static Cube<T> Span(Pos3<T> a, Pos3<T> b)
    {
        return new Cube<T>(
            Interval<T>.Span(a.X, b.X),
            Interval<T>.Span(a.Y, b.Y),
            Interval<T>.Span(a.Z, b.Z));
    }

    public static Cube<T> From(Pos3<T> start, Pos3<T> end)
    {
        return new Cube<T>(
            Interval<T>.From(start.X, end.X),
            Interval<T>.From(start.Y, end.Y),
            Interval<T>.From(start.Z, end.Z));
    }

    public Pos3<T> Min => new(MinX, MinY, MinZ);

    public Pos3<T> Max => new(MaxX, MaxY, MaxZ);

    public Pos3<T> End => new(EndX, EndY, EndZ);

    public Pos3<T> Size => new(XLength, YLength, ZLength);
    
    public bool Contains(Pos3<T> t)
    {
        return X.Contains(t.X)
               && Y.Contains(t.Y)
               && Z.Contains(t.Z);
    }

    public Cube<T> Intersect(Cube<T> other)
    {
        return new Cube<T>(
            X.Intersect(other.X),
            Y.Intersect(other.Y),
            Z.Intersect(other.Z));
    }

    public T XLength => X.Length;

    public T YLength => Y.Length;

    public T ZLength => Z.Length;

    public T MinX => X.Start;

    public T MinY => Y.Start;

    public T MinZ => Z.Start;

    public T MaxX => X.Last;

    public T MaxY => Y.Last;

    public T MaxZ => Z.Last;

    public T EndX => X.End;

    public T EndY => Y.End;

    public T EndZ => Z.End;

    public Enumerator GetEnumerator() => new(X, Y, Z);
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<Pos3<T>> IEnumerable<Pos3<T>>.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<Pos3<T>>
    {
        private readonly Interval<T> _x;
        private readonly Interval<T> _y;
        private readonly Interval<T> _z;
        private T _currentX;
        private T _currentY;
        private T _currentZ;

        public Enumerator(Interval<T> x, Interval<T> y, Interval<T> z)
        {
            _x = x;
            _y = y;
            _z = z;
            _currentX = x.Start - T.One;
            _currentY = y.Start;
            _currentZ = z.Start;
        }

        public Pos3<T> Current => new(_currentX, _currentY, _currentZ);

        public bool MoveNext()
        {
            if (++_currentX < _x.End) return true;
            _currentX = _x.Start;
            if (++_currentY < _y.End) return true;
            _currentY = _y.Start;
            return ++_currentZ < _z.End;
        }

        public void Reset() => throw new NotSupportedException();

        object IEnumerator.Current => Current;
        
        public void Dispose() { }
    }
}