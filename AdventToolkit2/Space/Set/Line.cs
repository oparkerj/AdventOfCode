using System.Numerics;
using AdventToolkit2.Space.Bound;
using AdventToolkit2.Space.Dimension;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Set;

/// <summary>
/// A sparse space where the position type is a number.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class Line<TNum, T, TDim> : SparseSpace<TNum, T, Interval<TNum>>, IAlignedSpace<TNum, T, TDim>
    where TNum : INumber<TNum>
    where TDim : IDimension<TNum>, new()
{
    public TDim Dimension { get; set; } = new();
}

/// <inheritdoc cref="Line{TNum, T, TDim}"/>
public class Line<TNum, T> : Line<TNum, T, LineDim<TNum>>
    where TNum : INumber<TNum>;

/// <inheritdoc cref="Line{TNum,T}"/>
public class Line<T> : Line<int, T>;