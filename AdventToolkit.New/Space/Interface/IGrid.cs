using System.Numerics;
using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// A grid represents two-dimensional storage.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public interface IGrid<TNum, T, TDim> :
    IAlignedSpace<Pos<TNum>, T, TDim>,
    IBounded<Pos<TNum>, Rect<TNum>>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>;