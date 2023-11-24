using System.Numerics;

namespace AdventToolkit.New.Extensions;

public static class NumberExtensions
{
    /// <summary>
    /// Convert a generic number to an integer.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int ToInt<T>(this T t)
        where T : INumberBase<T> => int.CreateTruncating(t);
    
    /// <summary>
    /// Convert a generic number to a long.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static long ToLong<T>(this T t)
        where T : INumberBase<T> => long.CreateTruncating(t);
}