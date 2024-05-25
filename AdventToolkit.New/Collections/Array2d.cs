using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Collections.Interface;
using AdventToolkit.New.Extensions;
using AdventToolkit.New.Space;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Collections;

/// <summary>
/// 2d wrapper over a 1d array.
/// This represents a slice of a 2d array. Use <see cref="FastArray2d{T}"/> to
/// represent the full 2d array.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Array2d<T> : IArray2dSlice<T, Array2d<T>>
{
    public readonly T[] Data;

    public readonly int FullWidth;
    public readonly int FullHeight;
    
    public readonly ValueRect<int> Bounds;

    public Array2d(T[] data, int width, int height, ValueRect<int> bounds)
    {
        Debug.Assert(data.Length == width * height);
        
        Data = data;
        FullWidth = width;
        FullHeight = height;
        Bounds = bounds;
    }

    public Array2d(T[] data, int width, int height) :
        this(data, width, height, new ValueRect<int>(width, height)) { }

    public Array2d(int width, int height) : 
        this(new T[width * height], width, height) { }

    public static implicit operator Array2d<T>(FastArray2d<T> array) => new(array.Data, array.Width, array.Height);

    public int Width => Bounds.Width;

    public int Height => Bounds.Height;

    /// <summary>
    /// Get the full size of the backing array. Which may be larger
    /// than this slice.
    /// </summary>
    public int FullCount => Data.Length;

    public int Count => Width * Height;

    public void Clear()
    {
        foreach (var i in GetIndexEnumerator())
        {
            Data[i] = default!;
        }
    }

    public bool Contains(T t)
    {
        foreach (var i in GetIndexEnumerator())
        {
            if (Equals(Data[i], t)) return true;
        }
        return false;
    }

    /// <summary>
    /// Get the index in the backing array from a 2d index.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Index(int x, int y)
    {
        return (y + Bounds.MinY) * FullWidth + x + Bounds.MinX;
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

    public Array2d<T> this[Interval<int> x, Interval<int> y] =>
        new(Data,
            FullWidth,
            FullHeight,
            new ValueRect<int>(
                Bounds.X.Slice(x.Start, x.Length),
                Bounds.Y.Slice(y.Start, y.Length)));

    public Array2d<T> this[Range x, Range y] => this[x.ToInterval(Width), y.ToInterval(Height)];
    
    public Array2d<T> this[int x, Range y] => this[x, y.ToInterval(Height)];
    
    public Array2d<T> this[Range x, int y] => this[x.ToInterval(Width), y];

    public Array2d<T> this[Rect<int> rect] => this[rect.X, rect.Y];

    public ArrayView<T> Row(int y) => new(Data, new Interval<int>(Index(0, y), Width));

    public Array2d<T> Rows(Interval<int> interval) => this[Bounds.X, interval];
    
    public Array2d<T> Rows(Range range) => Rows(range.ToInterval(FullHeight));

    public ArrayView<T> Col(int x) => new(Data, new Interval<int>(Index(x, 0), Height), FullWidth);

    public Array2d<T> Cols(Interval<int> interval) => this[interval, Bounds.Y];

    public Array2d<T> Cols(Range range) => Cols(range.ToInterval(FullWidth));

    public IndexEnumerator GetIndexEnumerator() => new(Bounds.MinX, Bounds.MinY, Width, Height, FullWidth);

    public Enumerator GetEnumerator() => new(Data, Bounds.MinX, Bounds.MinY, Width, Height, FullWidth);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public struct IndexEnumerator : IEnumerable<int>, IEnumerator<int>
    {
        public readonly int Stride;
        public readonly int Last;
        public readonly int WrapSubtract;
        
        public int Current { get; private set; }
        
        public IndexEnumerator(int x, int y, int width, int height, int arrayWidth) :
            this(y * arrayWidth + x,
                width == arrayWidth ? 1 : arrayWidth,
                (y + height - 1) * arrayWidth + x + width - 1,
                arrayWidth * height - 1)
        { }
        
        public IndexEnumerator(int start, int stride, int last, int wrapSubtract = 0)
        {
            Stride = stride;
            Last = last;
            WrapSubtract = wrapSubtract;
            Current = start - stride;
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (Current == Last) return false;
            if ((Current += Stride) > Last)
            {
                Current -= WrapSubtract;
                if (Current > Last)
                {
                    Current = Last;
                    return false;
                }
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<int> GetEnumerator() => this;

        public void Reset() => throw new NotSupportedException();

        public void Dispose() { }
    }

    public struct Enumerator : IEnumerable<T>, IEnumerator<T>
    {
        public readonly T[] Data;
        public IndexEnumerator IndexEnumerator;
        
        public Enumerator(T[] data, int x, int y, int width, int height, int arrayWidth) :
            this(data,
                y * arrayWidth + x,
                width == arrayWidth ? 1 : arrayWidth,
                (y + height - 1) * arrayWidth + x + width - 1,
                arrayWidth * height - 1)
        { }
        
        public Enumerator(T[] data, int start, int stride, int last, int wrapSubtract = 0)
        {
            Data = data;
            IndexEnumerator = new IndexEnumerator(start, stride, last, wrapSubtract);
        }

        public T Current => Data[IndexEnumerator.Current];

        object? IEnumerator.Current => Current;

        public bool MoveNext() => IndexEnumerator.MoveNext();

        public void Dispose() { }

        public void Reset() => throw new NotSupportedException();

        public Enumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    }
}