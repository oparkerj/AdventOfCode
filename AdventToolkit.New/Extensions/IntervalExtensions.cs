using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Extensions;

public static class IntervalExtensions
{
    /// <summary>
    /// Create an interval from a span which specifies the
    /// lower and upper bounds (inclusive).
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Interval<T> Create<T>(ReadOnlySpan<T> span)
        where T : INumber<T>
    {
        Debug.Assert(span.Length >= 2);
        return Interval<T>.From(span[0], span[1] + T.One);
    }
}