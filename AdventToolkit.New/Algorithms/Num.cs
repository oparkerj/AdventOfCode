using System.Numerics;

namespace AdventToolkit.New.Algorithms;

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
        return r < T.Zero ? r + mod : r;
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
        return num == T.Zero ? T.Zero : T.IsPositive(num) ? T.One : -T.One;
    }
}