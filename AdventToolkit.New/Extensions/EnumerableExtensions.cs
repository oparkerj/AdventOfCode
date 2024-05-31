using System.Collections;

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
}