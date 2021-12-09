namespace AdventToolkit.Collections.Graph;

public class Edge<T>
{
    public VertexBase<T> From { get; private set; }
    public VertexBase<T> To { get; private set; }

    public Edge(VertexBase<T> from, VertexBase<T> to)
    {
        From = from;
        To = to;
    }

    public virtual VertexBase<T> Other(VertexBase<T> vertex)
    {
        if (vertex == From) return To;
        if (vertex == To) return From;
        return null;
    }

    public TVertex OtherAs<TVertex>(TVertex vertex)
        where TVertex : VertexBase<T>
    {
        return Other(vertex) as TVertex;
    }

    public bool IsBetween(VertexBase<T> a, VertexBase<T> b)
    {
        return a == From && b == To || a == To && b == From;
    }

    public override string ToString() => $"{From} -- {To}";
}
    
public interface IEdgeData<out T>
{
    T Data { get; }
}

public class DataEdge<T, TData> : Edge<T>, IEdgeData<TData>
{
    public TData Data { get; }
        
    public DataEdge(VertexBase<T> from, VertexBase<T> to, TData data) : base(from, to)
    {
        Data = data;
    }
        
    public override string ToString()
    {
        return $"{base.ToString()} [label=\" {Data}\"];";
    }
}

public class DirectedEdge<T> : Edge<T>
{
    public DirectedEdge(VertexBase<T> from, VertexBase<T> to) : base(from, to) { }

    public override VertexBase<T> Other(VertexBase<T> vertexOld)
    {
        return vertexOld == From ? To : null;
    }

    public override string ToString() => $"{From} -> {To}";
}