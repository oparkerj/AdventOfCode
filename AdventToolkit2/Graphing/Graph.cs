using AdventToolkit2.Debugging;
using AdventToolkit2.Graphing.Interface;

namespace AdventToolkit2.Graphing;

public class Graph<TVertexModel, TVertex, THandle, TEdgeModel, TEdge>
    where TVertexModel : IVertexModel<TVertex, THandle>
    where TEdgeModel : class, IEdgeModel<TEdge, THandle>
{
    public readonly TVertexModel Vertices;

    public readonly TEdgeModel Edges;

    public Graph(TVertexModel vertices, TEdgeModel edges)
    {
        Vertices = vertices;
        Edges = edges;
    }

    public void LinkHandles(THandle source, THandle dest)
    {
        Edges.EnsureVertexCapacity(Vertices.Count);
        Edges.Link(source, dest);
    }

    public void Link(TVertex source, TVertex dest)
    {
        if (Vertices.TryLookupHandle(source, out var sourceHandle)
            && Vertices.TryLookupHandle(dest, out var destHandle))
        {
            LinkHandles(sourceHandle, destHandle);
        }
        else
        {
            Err.Argument("Vertex not found.");
        }
    }

    public void AddAll(IEnumerable<TEdge> edges)
    {
        Edges.EnsureVertexCapacity(Vertices.Count);
        foreach (var edge in edges)
        {
            Edges.Add(edge);
        }
    }
}