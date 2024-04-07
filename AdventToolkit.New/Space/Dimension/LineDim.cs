using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// Dimension for single-dimensional space.
/// </summary>
/// <typeparam name="TNum"></typeparam>
public class LineDim<TNum> : IDimension<TNum>
    where TNum : INumber<TNum>
{
    public static IEnumerable<TNum> GetNeighbors(TNum pos)
    {
        yield return pos + TNum.One;
        yield return pos - TNum.One;
    }
}