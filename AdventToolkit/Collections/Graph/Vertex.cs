using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Graph;

public static class Vertex
{
    public static Vertex<T, TEdge> Create<T, TEdge>(T value)
        where TEdge : Edge<T>
    {
        return new SimpleVertex<T, TEdge>(value);
    }
}

public abstract class VertexBase<T>
{
    public int Id { get; internal set; }

    public T Value { get; set; }
        
    protected VertexBase() { }

    protected VertexBase(T value)
    {
        Value = value;
    }
}
    
public abstract class Vertex<T, TEdge> : VertexBase<T>
    where TEdge : Edge<T>
{
    protected Vertex() { }

    protected Vertex(T value) : base(value) { }
        
    /// <summary>Number of edges on this vertex.</summary>
    public abstract int Count { get; }
        
    public abstract int NeighborCount { get; }

    /// <summary>
    /// Relevant edges; not every edge equals a neighbor, for
    /// example, directed edges. 
    /// </summary>
    public abstract IEnumerable<TEdge> NeighborEdges { get; }

    public IEnumerable<Vertex<T, TEdge>> Neighbors => NeighborEdges.Select(edge => edge.OtherAs(this));
    
    public IEnumerable<T> NeighborValues => Neighbors.Select(vertex => vertex.Value);

    public virtual IEnumerable<Vertex<T, TEdge>> Connected => Edges.Select(edge => edge.OtherAs(this));

    /// <summary>
    /// Every edge related to this vertex.
    /// </summary>
    public abstract IEnumerable<TEdge> Edges { get; }

    // Calls AddEdge, does not handle side effects
    public abstract void LinkTo(Vertex<T, TEdge> other, TEdge edge);

    public abstract void AddEdge(TEdge edge);

    public abstract void RemoveEdge(TEdge edge);

    public virtual bool ConnectedTo(Vertex<T, TEdge> other)
    {
        return Edges.Any(edge => edge.IsBetween(this, other));
    }
        
    public virtual void Disconnect()
    {
        foreach (var vertex in Connected.ToArray())
        {
            Unlink(vertex);
        }
    }

    /// <summary>
    /// Removes the first edge found that links this vertex and another.
    /// </summary>
    public virtual bool Unlink(Vertex<T, TEdge> other)
    {
        if (!Edges.Where(edge => edge.IsBetween(this, other)).First(out var otherEdge)) return false;
        RemoveEdge(otherEdge);
        return true;
    }
}
    
public class SimpleVertex<T, TEdge> : Vertex<T, TEdge>
    where TEdge : Edge<T>
{
    private List<TEdge> _edges = new();

    public SimpleVertex() { }

    public SimpleVertex(T value) : base(value) { }

    public override int Count => _edges.Count;

    public override int NeighborCount => NeighborEdges.Count();

    public override IEnumerable<TEdge> NeighborEdges => Edges.Where(edge => edge.Other(this) != null);

    public override IEnumerable<TEdge> Edges => _edges;

    public override void LinkTo(Vertex<T, TEdge> other, TEdge edge)
    {
        AddEdge(edge);
        other.AddEdge(edge);
    }

    public override void AddEdge(TEdge edge)
    {
        _edges.Add(edge);
    }

    public override void RemoveEdge(TEdge edge)
    {
        _edges.Remove(edge);
    }

    public override bool Unlink(Vertex<T, TEdge> other)
    {
        if (!_edges.First(edge => edge.IsBetween(this, other), out var edge)) return false;
        RemoveEdge(edge);
        return true;
    }

    public override string ToString() => $"\"{Value}\"";
}
    
public class Vertex<T> : SimpleVertex<T, Edge<T>> { }
    
public class DataVertex<T, TData> : SimpleVertex<T, DataEdge<T, TData>>
{
    public DataVertex() { }
        
    public DataVertex(T value) : base(value) { }
}

public class VertexInfo<T>
{
    public T Value { get; set; }
    public long Amount { get; set; } = 1;
    public T Child { get; set; }
    public long ChildAmount { get; set; } = 1;
    public List<T> Children { get; set; }
    public List<(T Child, long Amount)> ChildWeight { get; set; }
    public List<(long Amount, T Child)> WeightChild { get; set; }
}