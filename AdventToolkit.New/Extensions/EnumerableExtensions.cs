using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Calc;
using AdventToolkit.New.Debugging;

namespace AdventToolkit.New.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Provides disambiguation for when an object has its own enumerator.
    /// </summary>
    /// <param name="e"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Iter<T>(this IEnumerable<T> e) => e;

    /// <summary>
    /// Enumerable where the enumerator has already been created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly record struct EnumeratorWrap<T>(IEnumerator<T> Enumerator) : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => Enumerator;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Wrap an enumerator into an enumerable.
    /// </summary>
    /// <param name="e"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EnumeratorWrap<T> Wrap<T>(this IEnumerator<T> e) => new(e);

    /// <summary>
    /// Collect the items in a sequence.
    /// The main use of this method is when a collection needs to be
    /// modified while iterating over its values.
    /// </summary>
    /// <remarks>If the input is an array, then the span wraps the original array.
    /// This means modifications also apply to the array.</remarks>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Span<T> Collect<T>(this IEnumerable<T> items)
    {
        // Fast path for arrays
        if (items is T[] array) return array;
        
        // Fast path for collections
        if (items is ICollection<T> col)
        {
            var result = new T[col.Count];
            col.CopyTo(result, 0);
            return result;
        }

        var buffer = new T[4];
        var i = 0;
        
        foreach (var item in items)
        {
            if (i >= buffer.Length)
            {
                var size = buffer.Length * 2;
                if ((uint) size > Array.MaxLength)
                {
                    size = Array.MaxLength;
                }
                if (size < i + 1)
                {
                    size = i + 1;
                }
                var next = new T[size];
                Array.Copy(buffer, next, buffer.Length);
                buffer = next;
            }

            buffer[i++] = item;
        }

        return buffer.AsSpan(0, i);
    }
    
    /// <summary>
    /// Get the N smallest values from a sequence.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="count">Number of values to find.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] TakeMin<T>(this IEnumerable<T> source, int count)
    {
        var result = new T[count];
        Span<T> span = result;
        using var e = source.GetEnumerator();
        
        // Start by filling the array
        for (var i = 0; i < result.Length; i++)
        {
            if (!e.MoveNext()) Err.EndOfSequence();
            var search = span[..i].BinarySearch(e.Current, Compare<T>.DefaultComparer);
            
            // The result is either non-negative if the value is a duplicate or the bitwise
            // complement of the index where the value should be inserted.
            // The array is not filled yet so no need for upper bounds check.
            if (search < 0)
            {
                search = ~search;
            }
            result.InsertRight(search, e.Current);
        }

        // Keep track of the current largest minimum
        var maxMin = result[^1];
        
        while (e.MoveNext())
        {
            if (!Compare.Lt(e.Current, maxMin)) continue;

            // We have found a new minimum, insert it into the array.
            var search = span.BinarySearch(e.Current, Compare<T>.DefaultComparer);
            // We know that the value is less than the last item, so no need for
            // an upper bounds check.
            if (search < 0)
            {
                search = ~search;
            }
            result.InsertRight(search, e.Current);
            
            // Update the largest minimum
            maxMin = result[^1];
        }

        return result;
    }
    
    /// <summary>
    /// Get the N largest values from a sequence.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="count">Number of values to find.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] TakeMax<T>(this IEnumerable<T> source, int count)
    {
        var result = new T[count];
        Span<T> span = result;
        using var e = source.GetEnumerator();
        
        // Start by filling the array
        for (var i = 0; i < result.Length; i++)
        {
            if (!e.MoveNext()) Err.EndOfSequence();
            var search = span[..i].BinarySearch(e.Current, Compare<T>.DefaultComparer);
            
            // The result is either non-negative if the value is a duplicate or the bitwise
            // complement of the index where the value should be inserted.
            // The array is not filled yet so no need for upper bounds check.
            if (search < 0)
            {
                search = ~search;
            }
            result.InsertRight(search, e.Current);
        }

        // Keep track of the current smallest maximum
        var minMax = result[0];
        
        while (e.MoveNext())
        {
            if (!Compare.Gt(e.Current, minMax)) continue;

            // If we have found a new maximum, then insert it into the array
            var search = span.BinarySearch(e.Current, Compare<T>.DefaultComparer);
            if (search < 0)
            {
                search = ~search;
            }
            // The search result is the index of the first item greater than our value,
            // so we insert at the previous index. We know the value is greater than
            // the first item, so we can safely subtract one.
            result.InsertLeft(search - 1, e.Current);
            
            // Update the smallest maximum
            minMax = result[0];
        }

        return result;
    }

    /// <summary>
    /// This wrapper will create an <see cref="ExcludeEnumerator{T}"/>.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="start"></param>
    /// <param name="length"></param>
    /// <typeparam name="T"></typeparam>
    public readonly struct ExcludeEnumerable<T>(IEnumerable<T> source, int start, int length) : IEnumerable<T>
    {
        public ExcludeEnumerator<T> GetEnumerator() => new(source.GetEnumerator(), start, length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    }

    /// <inheritdoc cref="EnumerableExtensions.Exclude{T}"/>
    public struct ExcludeEnumerator<T>(IEnumerator<T> source, int start, int length) : IEnumerator<T>
    {
        private State _state = length == 0 ? State.After : State.Before;

        public T Current => source.Current;

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            switch (_state)
            {
                case State.Before when start > 0:
                    start--;
                    return source.MoveNext();
                case State.Before:
                    for (var i = 0; i < length; i++)
                    {
                        if (!source.MoveNext()) return false;
                    }
                    _state = State.After;
                    return source.MoveNext();
                default:
                    return source.MoveNext();
            }
        }

        public void Reset() => Err.NotSupported();

        public void Dispose() => source.Dispose();

        public enum State
        {
            Before,
            After,
        }
    }

    /// <summary>
    /// Excludes a contiguous range of items.
    /// It is not an error if the sequence is shorter than the exclude range.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="start">Index of the first item to exclude.</param>
    /// <param name="length">Number of items to exclude.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ExcludeEnumerable<T> Exclude<T>(this IEnumerable<T> source, int start, int length)
    {
        Debug.Assert(start >= 0);
        Debug.Assert(length >= 0);
        return new ExcludeEnumerable<T>(source, start, length);
    }
}