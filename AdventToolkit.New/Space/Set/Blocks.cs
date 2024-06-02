using System.Numerics;
using AdventToolkit.New.Space.Bound;
using AdventToolkit.New.Space.Dimension;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// Three-dimensional sparse space.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class Blocks<TNum, T, TDim> : SparseSpace<Pos3<TNum>, T, Cube<TNum>>, IAlignedSpace<Pos3<TNum>, T, TDim>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos3<TNum>>, new()
{
    public TDim Dimension { get; set; } = new();
}

/// <inheritdoc cref="Blocks{TNum,T,TDim}"/>
public class Blocks<TNum, T> : Blocks<TNum, T, PosAdjacent<Pos3<TNum>>>
    where TNum : INumber<TNum>;

/// <inheritdoc cref="Blocks{TNum,T,TDim}"/>
public class Blocks<T> : Blocks<int, T>;