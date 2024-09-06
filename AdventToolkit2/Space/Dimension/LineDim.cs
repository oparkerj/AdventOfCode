using System.Numerics;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Dimension;

/// <summary>
/// Dimension for single-dimensional space.
/// </summary>
/// <typeparam name="TNum"></typeparam>
public class LineDim<TNum> : IStaticDimension<LineDim<TNum>, TNum>
    where TNum : INumber<TNum>
{
    public static IEnumerable<TNum> GetNeighborsStatic(TNum pos)
    {
        yield return pos + TNum.One;
        yield return pos - TNum.One;
    }
}