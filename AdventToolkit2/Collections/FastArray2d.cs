using System.Collections;
using System.Diagnostics;
using AdventToolkit2.Collections.Interface;
using AdventToolkit2.Collections.Util;
using AdventToolkit2.Extensions;
using AdventToolkit2.Space;
using AdventToolkit2.Space.Bound;

namespace AdventToolkit2.Collections;

/// <summary>
/// 2d wrapper over a 1d array.
/// This represents the full backing array. Any slicing operations
/// will result in an <see cref="Array2d{T}"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct FastArray2d<T> : IArray2dSlice<T, Array2d<T>>
{
    public readonly T[] Data;

    public int Width { get; }

    public int Height { get; }

    public FastArray2d(T[] data, int width, int height)
    {
        Debug.Assert(data.Length == width * height);
        
        Data = data;
        Width = width;
        Height = height;
    }

    public FastArray2d(int width, int height) : this(width * height == 0 ? [] : new T[width * height], width, height) { }

    public int Count => Data.Length;
    
    public void Clear() => Array.Clear(Data);

    public bool Contains(T t) => Array.IndexOf(Data, t) > -1;

    public int Index(int x, int y)
    {
        Debug.Assert(x >= 0 && x < Width);
        Debug.Assert(y >= 0 && y < Height);
        return y * Width + x;
    }

    public T this[int x, int y]
    {
        get => Data[Index(x, y)];
        set => Data[Index(x, y)] = value;
    }

    /// <summary>
    /// Index the array with a position.
    /// </summary>
    /// <param name="pos"></param>
    public T this[Pos<int> pos]
    {
        get => this[pos.X, pos.Y];
        set => this[pos.X, pos.Y] = value;
    }

    public Array2d<T> this[Interval<int> x, Interval<int> y] => new(Data, Width, Height, new ValueRect<int>(x, y));

    public Array2d<T> this[Range x, Range y] => this[x.ToInterval(Width), y.ToInterval(Height)];

    public Array2d<T> this[int x, Range y] => this[x, y.ToInterval(Height)];

    public Array2d<T> this[Range x, int y] => this[x.ToInterval(Width), y];

    public Array2d<T> this[Rect<int> rect] => this[rect.X, rect.Y];

    /// <summary>
    /// Get a view of the full array.
    /// </summary>
    /// <returns></returns>
    public Array2d<T> View() => new(Data, Width, Height);

    public ArrayView<T> Row(int y)
    {
        Debug.Assert(y >= 0 && y < Height);
        return new ArrayView<T>(Data, new Interval<int>(y * Width, Width));
    }

    public Array2d<T> Rows(Interval<int> interval)
    {
        Debug.Assert(new Interval<int>(Height).Contains(interval));
        return new Array2d<T>(Data, Width, Height, new ValueRect<int>(new Interval<int>(Width), interval));
    }

    public Array2d<T> Rows(Range range) => Rows(range.ToInterval(Height));

    public ArrayView<T> Col(int x)
    {
        Debug.Assert(x >= 0 && x < Width);
        return new ArrayView<T>(Data, new Interval<int>(x, Height), Width);
    }

    public Array2d<T> Cols(Interval<int> interval)
    {
        Debug.Assert(new Interval<int>(Width).Contains(interval));
        return new Array2d<T>(Data, Width, Height, new ValueRect<int>(interval, new Interval<int>(Height)));
    }

    public Array2d<T> Cols(Range range) => Cols(range.ToInterval(Width));

    public Array2d<T>.Enumerator GetEnumerator() => new(Data, new IndexEnumerator(0, 1, Data.Length - 1));

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}