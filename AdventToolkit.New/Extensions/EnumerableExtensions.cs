using System.Collections;

namespace AdventToolkit.New.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Calls <see cref="IEnumerable{T}.GetEnumerator"/>.
    /// Mainly to get typed enumerator from an array instead of just <see cref="System.Collections.IEnumerator"/>.
    /// </summary>
    /// <param name="e"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerator<T> Enumerate<T>(this IEnumerable<T> e) => e.GetEnumerator();

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
}