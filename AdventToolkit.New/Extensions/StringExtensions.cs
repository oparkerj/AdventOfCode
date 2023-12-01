using System.Diagnostics;
using AdventToolkit.New.Data;

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