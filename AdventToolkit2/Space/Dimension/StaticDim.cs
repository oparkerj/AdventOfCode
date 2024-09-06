using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Dimension;

/// <summary>
/// Wrapper type for instances of <see cref="IStaticDimension{TSelf,T}"/> aimed
/// at reducing allocation size.
/// </summary>
/// <typeparam name="TDim"></typeparam>
/// <typeparam name="T"></typeparam>
public struct StaticDim<TDim, T> : IDimension<T>
    where TDim : IStaticDimension<TDim, T>
{
    public IEnumerable<T> GetNeighbors(T pos) => TDim.GetNeighborsStatic(pos);
}