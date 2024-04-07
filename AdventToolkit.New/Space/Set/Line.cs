using System.Numerics;
using AdventToolkit.New.Space.Dimension;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// A sparse space where the position type is a number.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class Line<TNum, T, TDim> : SparseSpace<TNum, T>, IAlignedSpace<TNum, T, TDim>
    where TNum : INumber<TNum>
    where TDim : IDimension<TNum>;

/// <inheritdoc cref="Line{TNum, T, TDim}"/>
public class Line<TNum, T> : Line<TNum, T, LineDim<TNum>>
    where TNum : INumber<TNum>;

/// <inheritdoc cref="Line{TNum,T}"/>
public class Line<T> : Line<int, T>;