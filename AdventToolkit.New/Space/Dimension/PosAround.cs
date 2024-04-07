using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// This dimension will use spaces around a position rather than
/// just adjacent positions.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TPos"></typeparam>
public class PosAround<T, TPos> : IDimension<TPos>
    where TPos : IPos<TPos, T>
    where T : notnull
{
    public static IEnumerable<TPos> GetNeighbors(TPos pos) => pos.Around();
}