namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// Defines how to get adjacent positions in a space.
/// </summary>
/// <typeparam name="T">Space position type.</typeparam>
public interface IDimension<T>
{
    /// <summary>
    /// Get neighbor values.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerable<T> GetNeighbors(T pos);
}