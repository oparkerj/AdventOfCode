using System.Numerics;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// Two-dimensional sparse space.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class Grid<TNum, T, TDim> : SparseSpace<Pos<TNum>, T>, IAlignedSpace<Pos<TNum>, T, TDim>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>;

/// <inheritdoc cref="Grid{TNum,T,TDim}"/>
public class Grid<TNum, T> : Grid<TNum, T, Pos<TNum>>
    where TNum : INumber<TNum>;

/// <inheritdoc cref="Grid{TNum,T,TDim}"/>
public class Grid<T> : Grid<int, T>;