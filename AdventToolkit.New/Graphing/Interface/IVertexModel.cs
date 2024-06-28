namespace AdventToolkit.New.Graphing.Interface;

/// <summary>
/// Defines the way vertices are stored in a graph.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="THandle"></typeparam>
public interface IVertexModel<T, THandle> : IEnumerable<T>
{
    /// <summary>
    /// Number of vertices.
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Add a vertex.
    /// </summary>
    /// <param name="t"></param>
    /// <returns>Unique handle associated with the vertex.</returns>
    THandle Add(T t);

    /// <summary>
    /// Remove a vertex.
    /// If this applies to multiple vertices, this will only remove one.
    /// </summary>
    /// <param name="t"></param>
    void Remove(T t);

    /// <summary>
    /// Remove the vertex with the associated handle.
    /// </summary>
    /// <param name="handle"></param>
    void RemoveHandle(THandle handle);

    /// <summary>
    /// Get a handle associated with the vertex.
    /// If there are multiple handles for the given value, this may
    /// return any of them.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    bool TryLookupHandle(T t, out THandle handle);

    /// <summary>
    /// Get all handles associated with the vertex value.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    IEnumerable<THandle> LookupAll(T t);

    /// <summary>
    /// Check if a vertex exists.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    bool Contains(T t);

    /// <summary>
    /// Check if a particular handle exists.
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    bool ContainsHandle(THandle handle);

    /// <summary>
    /// Remove all vertices.
    /// Clearing the model will invalidate any existing handles.
    /// </summary>
    void Clear();
}