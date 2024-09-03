using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Data;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Extensions;

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