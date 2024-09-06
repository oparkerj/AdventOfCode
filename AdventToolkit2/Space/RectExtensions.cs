using System.Numerics;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space;

public static class RectExtensions
{
    /// <summary>
    /// Get the area of the rect.
    /// </summary>
    /// <param name="rect"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TNum"></typeparam>
    /// <returns></returns>
    public static TNum Area<T, TNum>(this IRect<T, TNum> rect)
        where T : IRect<T, TNum>
        where TNum : INumber<TNum> =>
        rect.Width * rect.Height;
}