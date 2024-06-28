namespace AdventToolkit.New.Graphing.Interface;

/// <summary>
/// Defines the way edges are stored in a graph.
/// </summary>
/// <typeparam name="TEdge"></typeparam>
/// <typeparam name="TVertex"></typeparam>
public interface IEdgeModel<TEdge, TVertex> : IEnumerable<TEdge>
{
    /// <summary>
    /// Get the number of edges.
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// The number of distinct vertices that can currently be connected without resizing.
    /// This does not indicate edge capacity.
    /// </summary>
    int VertexCapacity { get; set; }

    /// <summary>
    /// Make sure the model is prepared to support at least a given number
    /// of vertices being connected.
    /// </summary>
    /// <param name="capacity"></param>
    void EnsureVertexCapacity(int capacity);

    /// <summary>
    /// Add an edge.
    /// </summary>
    /// <param name="edge"></param>
    void Add(TEdge edge);

    /// <summary>
    /// Remove an edge.
    /// </summary>
    /// <param name="edge"></param>
    void Remove(TEdge edge);

    /// <summary>
    /// Check if an edge exists.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    bool Contains(TEdge edge);

    /// <summary>
    /// Remove all edges.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Add a link between two vertices.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    void Link(TVertex source, TVertex dest);

    /// <summary>
    /// Remove a link between two vertices.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    void Unlink(TVertex source, TVertex dest);

    /// <summary>
    /// Remove all links for a vertex.
    /// </summary>
    /// <param name="vertex"></param>
    void Unlink(TVertex vertex);

    /// <summary>
    /// Check if vertices are linked.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    bool IsLinked(TVertex source, TVertex dest);

    /// <summary>
    /// Try to get a link between vertices.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="edge"></param>
    /// <returns></returns>
    bool TryGetLink(TVertex source, TVertex dest, out TEdge edge);

    /// <summary>
    /// Get all links between two vertices.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    IEnumerable<TEdge> GetLinks(TVertex source, TVertex dest);

    /// <summary>
    /// Get all links for a vertex.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    IEnumerable<TEdge> GetLinks(TVertex vertex);

    /// <summary>
    /// Get all edges that can be used to get from another
    /// vertex to this one.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    IEnumerable<TEdge> GetIncoming(TVertex vertex);
    
    /// <summary>
    /// Get all edges that can be used to go from this vertex
    /// to another one.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    IEnumerable<TEdge> GetOutgoing(TVertex vertex);
}