using System.Numerics;

namespace AdventToolkit.New.Data.Space;

/// <summary>
/// A sparse space where the position type is a number.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
public class Line<TNum, T> : SparseSpace<TNum, T>
    where TNum : INumber<TNum>
{
    public override IEnumerable<TNum> GetNeighbors(TNum pos)
    {
        yield return pos + TNum.One;
        yield return pos - TNum.One;
    }
}

/// <inheritdoc cref="Line{TNum,T}"/>
public class Line<T> : Line<int, T> { }