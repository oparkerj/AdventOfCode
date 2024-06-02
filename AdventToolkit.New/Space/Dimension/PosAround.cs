using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// This dimension will use spaces around a position rather than
/// just adjacent positions.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct PosAround<T> : IStaticDimension<PosAround<T>, T>
    where T : IPos<T>
{
    public static IEnumerable<T> GetNeighborsStatic(T pos) => pos.Around();
}