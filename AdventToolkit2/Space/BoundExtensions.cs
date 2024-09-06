using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space;

public static class BoundExtensions
{
    /// <summary>
    /// Filter a sequence on items which are contained within a bound.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="bound"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBound"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Within<T, TBound>(this IEnumerable<T> source, TBound bound)
        where TBound : IBound<TBound, T>
    {
        foreach (var t in source)
        {
            if (bound.Contains(t)) yield return t;
        }
    }
}