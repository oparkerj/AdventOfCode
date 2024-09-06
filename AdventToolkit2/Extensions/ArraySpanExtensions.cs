using System.Diagnostics;

namespace AdventToolkit2.Extensions;

public static class ArraySpanExtensions
{
    /// <summary>
    /// Insert an item into an array. The element at the index will be shifted to the right.
    /// This will cause the last item to be dropped from the array.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="index">Insertion index.</param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertRight<T>(this Span<T> span, int index, T item)
    {
        Debug.Assert(index >= 0 && index < span.Length);
        span[index..^1].CopyTo(span[(index + 1)..]);
        span[index] = item;
    }
    
    /// <inheritdoc cref="InsertRight{T}(System.Span{T},int,T)"/>
    public static void InsertRight<T>(this T[] array, int index, T item) => InsertRight(array.AsSpan(), index, item);

    /// <summary>
    /// Insert an item into an array. The element at the index will be shifted to the left.
    /// This will cause the first item to be dropped from the array.
    /// </summary>
    /// <param name="span"></param>
    /// <param name="index">Insertion index.</param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertLeft<T>(this Span<T> span, int index, T item)
    {
        Debug.Assert(index >= 0 && index < span.Length);
        span[1..(index + 1)].CopyTo(span);
        span[index] = item;
    }

    /// <inheritdoc cref="InsertLeft{T}(System.Span{T},int,T)"/>
    public static void InsertLeft<T>(this T[] array, int index, T item) => InsertLeft(array.AsSpan(), index, item);
}