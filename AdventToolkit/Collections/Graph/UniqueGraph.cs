using System;
using System.Collections.Generic;

namespace AdventToolkit.Collections.Graph;

public class UniqueGraph<T, TVertex, TEdge> : Graph<T, TVertex, TEdge>
    where TVertex : Vertex<T, TEdge>
    where TEdge : Edge<T>
{
    private new readonly Dictionary<T, TVertex> _vertices = new();

    public override void AddVertex(TVertex vertex)
    {
        base.AddVertex(vertex);
        _vertices[vertex.Value] = vertex;
    }

    public override void RemoveVertex(TVertex vertex)
    {
        base.RemoveVertex(vertex);
        _vertices.Remove(vertex.Value);
    }

    public TVertex this[T val] => Get(val);
        
    public TVertex Get(T val) => _vertices[val];

    public bool TryGet(T val, out TVertex vertex)
    {
        return _vertices.TryGetValue(val, out vertex);
    }

    public TVertex GetOrCreate(T val, Func<T, TVertex> cons)
    {
        if (_vertices.TryGetValue(val, out var vertex)) return vertex;
        vertex = cons(val);
        AddVertex(vertex);
        return vertex;
    }
}
    
public class UniqueGraph<T> : UniqueGraph<T, Vertex<T, Edge<T>>, Edge<T>> { }

public class UniqueDataGraph<T, TData> : UniqueGraph<T, DataVertex<T, TData>, DataEdge<T, TData>> { }

public class UniqueDigraph<T> : UniqueGraph<T, Vertex<T, DirectedEdge<T>>, DirectedEdge<T>> { }

public class UniqueDataDigraph<T, TData> : UniqueGraph<T, Vertex<T, DirectedDataEdge<T, TData>>, DirectedDataEdge<T, TData>> { }