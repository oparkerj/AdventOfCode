using System.Numerics;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Extensions;

public static class RangeIndexExtensions
{
    /// <summary>
    /// Get an index offset as a different number type.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="length"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetOffset<T>(this Index i, T length)
        where T : INumber<T>
    {
        if (i.IsFromEnd) return length - T.CreateTruncating(i.Value);
        return T.CreateTruncating(i.Value);
    }

    /// <summary>
    /// Get range offsets as a different number type.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="length"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static (T Start, T End) GetOffsets<T>(this Range r, T length)
        where T : INumber<T>
    {
        return (r.Start.GetOffset(length), r.End.GetOffset(length));
    }

    /// <summary>
    /// Convert a range to an interval.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Interval<int> ToInterval(this Range r, int length)
    {
        var (start, count) = r.GetOffsetAndLength(length);
        return new Interval<int>(start, count);
    }
    
    /// <summary>
    /// Convert a range to an interval.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="length"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Interval<T> ToInterval<T>(this Range r, T length)
        where T : INumber<T>
    {
        var start = r.Start.GetOffset(length);
        var end = r.End.GetOffset(length);
        return Interval<T>.From(start, end);
    }
}