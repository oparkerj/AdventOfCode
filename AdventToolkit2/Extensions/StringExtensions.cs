using System.Diagnostics;
using System.Numerics;
using AdventToolkit2.Data;
using AdventToolkit2.Space.Bound;

namespace AdventToolkit2.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Slice a string using an interval.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static ReadOnlySpan<char> Slice(this string s, Interval<int> interval)
    {
        Debug.Assert(interval.Start >= 0);
        Debug.Assert(interval.Length >= 0);
        Debug.Assert(interval.End <= s.Length);
        return s.AsSpan(interval.Start, interval.Length);
    }

    /// <inheritdoc cref="Slice"/>
    public static ReadOnlySpan<char> Slice<T>(this string s, Interval<T> interval)
        where T : INumber<T>
    {
        return s.Slice(interval.As<int>());
    }

    /// <summary>
    /// Create a view of a string.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static Str View(this string s, Interval<int> interval)
    {
        Debug.Assert(interval.Start >= 0);
        Debug.Assert(interval.Length >= 0);
        Debug.Assert(interval.End <= s.Length);
        return new Str(s, interval);
    }
}