using System.Numerics;

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
}