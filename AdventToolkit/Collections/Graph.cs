using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class Graph<T, TVertex, TEdge> : IEnumerable<TVertex>
        where TVertex : Vertex<T, TEdge>
        where TEdge : Edge<T>
    {
        private int _counter;
        
        internal readonly Dictionary<int, TVertex> _vertices = new();

        public int Count => _vertices.Count;

        public TVertex this[int id] => _vertices[id];

        public virtual void AddVertex(TVertex vertex)
        {
            vertex.Id = _counter++;
            _vertices[vertex.Id] = vertex;
        }

        public virtual void RemoveVertex(TVertex vertex)
        {
            if (_vertices.TryGetValue(vertex.Id, out var v) && v != vertex) return;
            vertex.Disconnect();
            _vertices.Remove(vertex.Id);
        }

        public bool Lookup(int id, out TVertex vertex) => _vertices.TryGetValue(id, out vertex);

        public IEnumerable<T> Values => this.Select(vertex => vertex.Value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TVertex> GetEnumerator() => _vertices.Values.GetEnumerator();

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append(typeof(TEdge) == typeof(DirectedEdge<T>) ? "digraph G {\n" : "graph G {\n");
            foreach (var vertex in _vertices.Values)
            {
                b.Append(vertex).Append('\n');
            }
            foreach (var edge in _vertices.Values.SelectMany(vertex => vertex.Edges).Distinct())
            {
                b.Append(edge).Append('\n');
            }
            b.Append("}\n");
            return b.ToString();
        }
    }

    // Some shorthands
    public class Graph<T> : Graph<T, Vertex<T, Edge<T>>, Edge<T>> { }
    
    public class DataGraph<T, TData> : Graph<T, Vertex<T, DataEdge<T, TData>>, DataEdge<T, TData>> { }

    public class Digraph<T> : Graph<T, Vertex<T, DirectedEdge<T>>, DirectedEdge<T>> { }

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

        public override int NeighborCount => Count;

        public override IEnumerable<TEdge> NeighborEdges => Edges;

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

    public interface IEdgeData<T>
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
    
    public class UniqueDataGraph<T, TData> : UniqueGraph<T, DataVertex<T, TData>, DataEdge<T, TData>> { }

    public class UniqueDigraph<T> : UniqueGraph<T, Vertex<T, DirectedEdge<T>>, DirectedEdge<T>> { }
}