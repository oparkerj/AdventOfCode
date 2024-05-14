using System.Collections;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Bound;

/// <summary>
/// This bound is always empty and every operation is effectively a no-op.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Unbounded<T> : IBound<Unbounded<T>, T>
{
    public static bool operator ==(Unbounded<T> a, Unbounded<T> b) => true;
    
    public static bool operator !=(Unbounded<T> a, Unbounded<T> b) => false;
    
    public static Unbounded<T> Empty => [];

    public static Unbounded<T> Span(T a, T b) => Empty;

    public static Unbounded<T> From(T start, T end) => Empty;

    public static implicit operator Unbounded<T>(T num) => Empty;
    
    public T Min => throw new NotSupportedException();

    public T Max => throw new NotSupportedException();
    
    public T End => throw new NotSupportedException();
    
    public T Size => throw new NotSupportedException();
    
    public bool Contains(T t) => false;

    public bool Contains(Unbounded<T> t) => false;

    public Unbounded<T> Add(T num) => this;

    public Unbounded<T> Intersect(Unbounded<T> other) => this;
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        yield break;
    }

    public override bool Equals(object? obj) => obj is Unbounded<T>;

    public override int GetHashCode() => 0;

    public override string ToString() => "[]";
}