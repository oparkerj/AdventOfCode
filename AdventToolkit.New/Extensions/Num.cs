using System.Numerics;

namespace AdventToolkit.New.Extensions;

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
}