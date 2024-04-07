namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// An aligned space adds the ability to get the locations of adjacent positions.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
public interface IAlignedSpace<TPos, TVal> : ISpace<TPos, TVal>
{
    /// <summary>
    /// Get adjacent positions.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerable<TPos> GetNeighbors(TPos pos);
}

/// <summary>
/// An aligned space that uses the given dimension by default to get adjacent positions.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
/// <typeparam name="TDim"></typeparam>
public interface IAlignedSpace<TPos, TVal, TDim> : IAlignedSpace<TPos, TVal>
    where TDim : IDimension<TPos>
{
    IEnumerable<TPos> IAlignedSpace<TPos, TVal>.GetNeighbors(TPos pos) => TDim.GetNeighbors(pos);
}