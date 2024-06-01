using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Debugging;

namespace AdventToolkit.New.Collections.Util;

/// <summary>
/// An enumerator for an array where the content may need to
/// wrap around the end of the array.
/// </summary>
/// <param name="data"></param>
/// <param name="start"></param>
/// <param name="last"></param>
/// <typeparam name="T"></typeparam>
public struct ArrayEnumerator<T>(T[] data, int start, int last) : IEnumerable<T>, IEnumerator<T>
{
    public readonly T[] Data = data;
    public readonly int Last = last;
    public int Index = start;

    /// <summary>
    /// Create an array enumerator from a start and length.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static ArrayEnumerator<T> From(T[] data, int start, int length)
    {
        Debug.Assert(length >= 0 && length <= data.Length);
        Debug.Assert(start >= 0 && start < data.Length);

        var last = start + length - 1;
        if (last >= data.Length)
        {
            last -= data.Length;
        }
        return new ArrayEnumerator<T>(data, start, last);
    }

    /// <summary>
    /// Get an enumerator over the entire array starting at the given index.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="start"></param>
    /// <returns></returns>
    public static ArrayEnumerator<T> From(T[] data, int start) => From(data, start, data.Length);

    public T Current => Data[Index];

    object? IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (Index == Last) return false;
        if (++Index >= Data.Length)
        {
            Index = 0;
        }
        return true;
    }

    public void Dispose() { }

    public void Reset() => Err.NotSupported();

    public ArrayEnumerator<T> GetEnumerator() => this;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}