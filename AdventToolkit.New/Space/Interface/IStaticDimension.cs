namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// A dimension which does not require any state.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="T"></typeparam>
public interface IStaticDimension<TSelf, T> : IDimension<T>
    where TSelf : IStaticDimension<TSelf, T>
{
    IEnumerable<T> IDimension<T>.GetNeighbors(T pos) => TSelf.GetNeighborsStatic(pos);

    /// <inheritdoc cref="IDimension{T}.GetNeighbors"/>
    static abstract IEnumerable<T> GetNeighborsStatic(T pos);
}