namespace AdventToolkit2.Space.Interface;

/// <summary>
/// Defines the sides of a position type.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSide"></typeparam>
public interface IDimSides<T, TSide>
{
    /// <summary>
    /// Get the sides available for the given position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerable<TSide> GetSides(T pos);

    /// <summary>
    /// Get the position on a particular side.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    T GetSide(T pos, TSide side);

    /// <summary>
    /// Get the side from two positions.
    /// This returns a side if the other position is exactly in line
    /// with one of the sides.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="other"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    bool TryLookupSide(T pos, T other, out TSide side);

    /// <summary>
    /// Get the side from two positions.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="other"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    bool TryLookupSideExact(T pos, T other, out TSide side);
}