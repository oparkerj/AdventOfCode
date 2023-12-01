using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Algorithms;

public static class NumVal<T>
    where T : INumber<T>
{
    /// <summary>
    /// Equivalent to <see cref="INumberBase{T}.One"/>
    /// </summary>
    public static readonly T One = T.One;

    /// <summary>
    /// The value of adding <see cref="One"/> to itself.
    /// </summary>
    public static readonly T Two = One + One;
}

/// <summary>
/// Number extensions
/// </summary>
public static class Num
{
    /// <summary>
    /// Performs the mod operation.
    /// This method always produces non-negative results.
    /// </summary>
    /// <param name="num">Numerator</param>
    /// <param name="mod">Denominator</param>
    /// <typeparam name="T">Number type</typeparam>
    /// <returns>Modulus of the operands.</returns>
    public static T Mod<T>(this T num, T mod)
        where T : INumber<T>
    {
        var r = num % mod;
        return T.IsNegative(r) ? r + mod : r;
    }

    /// <summary>
    /// Similar to <see cref="Math.Sign(int)"/> but the return
    /// type matches the input type.
    /// </summary>
    /// <param name="num"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns><see cref="INumberBase{T}.Zero"/> if the value is zero.
    /// Otherwise, positive or negative <see cref="INumberBase{T}.One"/></returns>
    public static T Sign<T>(this T num)
        where T : INumber<T>
    {
        return T.IsZero(num) ? T.Zero : T.IsPositive(num) ? T.One : -T.One;
    }

    /// <summary>
    /// Sum the values between min and max (inclusive).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T SumSpan<T>(T min, T max)
        where T : INumber<T>
    {
        Debug.Assert(min <= max);
        return (max - min + T.One) * ((max + min) / NumVal<T>.Two);
    }

    /// <summary>
    /// Sum the values in the interval.
    /// </summary>
    /// <param name="interval"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Sum<T>(this Interval<T> interval)
        where T : INumber<T>
    {
        Debug.Assert(interval.Length >= T.Zero);
        return interval.Length * ((interval.Min + interval.Last) / NumVal<T>.Two);
    }
}