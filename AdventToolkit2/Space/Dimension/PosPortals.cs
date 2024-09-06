using System.Numerics;
using AdventToolkit2.Collections;
using AdventToolkit2.Space.Direction;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Dimension;

/// <summary>
/// Alias to set up a portal dimension for <see cref="Pos{T}"/>.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class PosPortals<TNum, TDim> : PortalDim<Pos<TNum>, TDim, PosSide4<TNum>, Side, EnumDict<Side, Pos<TNum>>>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>
{
    public PosPortals(TDim dimension, PosSide4<TNum> sides) : base(dimension, sides) { }

    public PosPortals(Dictionary<Pos<TNum>, EnumDict<Side, Pos<TNum>>> sideMap, TDim dimension, PosSide4<TNum> sides)
        : base(sideMap, dimension, sides) { }
}