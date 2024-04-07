namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// Defines how to get adjacent positions in a space.
/// </summary>
/// <typeparam name="T">Space position type.</typeparam>
public interface IDimension<T>
{
    /// <summary>
    /// Get adjacent values.
    /// </summary>
    /// <param name="pos">Current position.</param>
    /// <returns></returns>
    static abstract IEnumerable<T> GetNeighbors(T pos);
}