using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Dimension;

/// <summary>
/// This dimension will return adjacent positions
/// </summary>
/// <typeparam name="T"></typeparam>
public struct PosAdjacent<T> : IStaticDimension<PosAdjacent<T>, T>
    where T : IPos<T>
{
    public static IEnumerable<T> GetNeighborsStatic(T pos) => pos.Adjacent();
}