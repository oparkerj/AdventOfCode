using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Debugging;

namespace AdventToolkit.New.Collections.Util;

/// <summary>
/// This enumerates the indices that would be accessed when enumerating
/// over a section of an array.
/// </summary>
/// <param name="start"></param>
/// <param name="stride"></param>
/// <param name="last"></param>
/// <param name="wrapSubtract"></param>
public struct IndexEnumerator(int start, int stride, int last, int wrapSubtract = 0)
    : IEnumerable<int>, IEnumerator<int>
{
    public readonly int Stride = stride;
    public readonly int Last = last;
    public readonly int WrapSubtract = wrapSubtract;
        
    public int Current { get; private set; } = start - stride;

    /// <summary>
    /// Create an index enumerator given the desired rect bounds.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="arrayWidth"></param>
    /// <returns></returns>
    public static IndexEnumerator From(int x, int y, int width, int height, int arrayWidth)
    {
        Debug.Assert(x >= 0);
        Debug.Assert(y >= 0);
        Debug.Assert(width >= 0);
        Debug.Assert(height >= 0);
        
        var start = y * arrayWidth + x;
        var stride = width == arrayWidth ? 1 : arrayWidth;
        // If the rect is empty, then the last value should be equal to the initial value
        var last = width == 0 || height == 0 ? start - stride : (y + height - 1) * arrayWidth + x + width - 1;
        var wrapSubtract = arrayWidth * height - 1;

        return new IndexEnumerator(start, stride, last, wrapSubtract);
    }

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (Current == Last) return false;
        if ((Current += Stride) > Last)
        {
            Current -= WrapSubtract;
            Debug.Assert(Current <= Last);
        }
        return true;
    }

    public IndexEnumerator GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<int> IEnumerable<int>.GetEnumerator() => GetEnumerator();

    public void Reset() => Err.NotSupported();

    public void Dispose() { }
}