using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Debugging;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Collections;

/// <summary>
/// Represents a view of a 1d array.
/// The <see cref="Stride"/> value can be used to access every Nth item
/// in the array.
/// Index 0 in the view corresponds to Data[Range.Start]
/// The last item in the view is Data[Range.Start + (Range.Length - 1) * Stride].
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct ArrayView<T> : IEnumerable<T>
{
    public readonly T[] Data;

    /// <summary>
    /// 
    /// </summary>
    public readonly Interval<int> Range;
    public readonly int Stride;

    /// <summary>
    /// Create a view of an array.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="range"></param>
    /// <param name="stride"></param>
    public ArrayView(T[] data, Interval<int> range, int stride = 1)
    {
        Debug.Assert(range.Start >= 0);
        Debug.Assert(range.Start + range.Length * stride <= data.Length);
        
        Data = data;
        Range = range;
        Stride = stride;
    }

    public ArrayView(T[] data) : this(data, new Interval<int>(data.Length)) { }

    public static implicit operator ArrayView<T>(T[] array) => new(array);

    /// <summary>
    /// Length of this view.
    /// </summary>
    public int Length => Range.Length;

    /// <summary>
    /// Index into the view.
    /// </summary>
    /// <param name="i"></param>
    public T this[int i]
    {
        get
        {
            Debug.Assert(i >= 0 && i < Range.Length);
            return Data[Range.Start + i * Stride];
        }
        set
        {
            Debug.Assert(i >= 0 && i < Range.Length);
            Data[Range.Start + i * Stride] = value;
        }
    }

    /// <summary>
    /// Slice the view.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public ArrayView<T> Slice(int start, int length)
    {
        return new ArrayView<T>(Data, Range.Slice(start, length), Stride);
    }

    public Enumerator GetEnumerator() => new(Data, Range, Stride);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public struct Enumerator(T[] data, Interval<int> range, int stride) : IEnumerator<T>
    {
        public readonly T[] Data = data;
        public readonly int Stride = stride;
        
        // This still works for length = 0.
        // Index is initialized to one step before the start.
        // For length = 0, Last is also set to the step before the start,
        // which will cause the first move to return false.
        public readonly int Last = range.Start + (range.Length - 1) * stride;
        public int Index = range.Start - stride;

        public T Current { get; private set; } = default!;

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (Index >= Last) return false;
            Current = Data[Index += Stride];
            return true;
        }

        public void Dispose() { }

        public void Reset() => Err.NotSupported();
    }
}