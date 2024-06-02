using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Dimension;

/// <summary>
/// This dimension will return adjacent positions
/// </summary>
/// <typeparam name="T"></typeparam>
public struct PosAdjacent<T> : IStaticDimension<PosAround<T>, T>
    where T : IPos<T>
{
    public static IEnumerable<T> GetNeighborsStatic(T pos) => pos.Adjacent();
}