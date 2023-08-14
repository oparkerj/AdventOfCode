using System.Numerics;

namespace AdventToolkit.New.Extensions;

public static class Num
{
    public static T Mod<T>(this T num, T mod)
        where T : INumber<T>
    {
        var r = num % mod;
        return r < T.Zero ? r + mod : r;
    }
}