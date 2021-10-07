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
            foreach (var edge in vertex.Edges.ToArray())
            {
                edge.Unlink();
            }
            _vertices.Remove(vertex.Id);
        }

        public bool Lookup(int id, out TVertex vertex) => _vertices.TryGetValue(id, out vertex);

        public IEnumerable<T> Values => this.Select(vertex => vertex.Value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TVertex> GetEnumerator() => _vertices.Values.GetEnumerator();

        public override string ToString()
        {
            var b = new StringBuilder();
            b.Append(typeof(TEdge) == typeof(DirectedEdgeOld<T>) ? "digraph G {\n" : "graph G {\n");
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

    public class Graph<T> : Graph<T, Vertex<T, Edge<T>>, Edge<T>> { }

    public abstract class Vertex<T, TEdge>
    {
        public int Id { get; internal set; }

        public T Value { get; set; }

        protected Vertex(T value)
        {
            Value = value;
        }

        /// <summary>Number of edges on this vertex.</summary>
        public abstract int Count { get; }

        /// <summary>
        /// Relevant neighbors; not every edge equals a neighbor, for
        /// example, directed edges. 
        /// </summary>
        public abstract IEnumerable<Vertex<T>> Neighbors { get; }
        
        /// <summary>
        /// Every edge related to this vertex.
        /// </summary>
        public abstract IEnumerable<Edge<T>> Edges { get; }

        public abstract void AddEdge(Edge<T> edge);

        public abstract void RemoveEdge(Edge<T> edge);

        public virtual bool ConnectedTo(Vertex<T> other)
        {
            return Edges.Any(edge => edge.IsBetween(this, other));
        }

        /// <summary>
        /// Removes the first edge found that links this vertex and another.
        /// </summary>
        public virtual bool Unlink(Vertex<T> other)
        {
            if (!Edges.Where(edge => edge.IsBetween(this, other)).First(out var otherEdge)) return false;
            RemoveEdge(otherEdge);
            return true;
        }
    }

    public class SimpleVertex<T> : Vertex<T>
    {
        private List<Edge<T>> _edges = new();

        public SimpleVertex(T value) : base(value) { }

        public override int Count => _edges.Count;

        public override IEnumerable<Vertex<T>> Neighbors => _edges.Select(edge => edge.Other(this));
        
        public override IEnumerable<Edge<T>> Edges => _edges;

        public override void AddEdge(Edge<T> edge)
        {
            _edges.Add(edge);
        }

        public override void RemoveEdge(Edge<T> edge)
        {
            _edges.Remove(edge);
        }

        public override bool Unlink(Vertex<T> other)
        {
            var index = _edges.FindIndex(edge => edge.IsBetween(this, other));
            if (index < 0) return false;
            _edges.RemoveAt(index);
            return true;
        }

        public override string ToString() => $"\"{Value}\"";
    }

    public class Edge<T>
    {
        public Vertex<T> From { get; private set; }
        public Vertex<T> To { get; private set; }

        public Edge(Vertex<T> from, Vertex<T> to)
        {
            From = from;
            To = to;
        }

        public virtual Vertex<T> Other(Vertex<T> vertex)
        {
            if (vertex == From) return To;
            if (vertex == To) return From;
            return null;
        }

        public TVertex OtherAs<TVertex>(TVertex vertex)
            where TVertex : Vertex<T>
        {
            return Other(vertex) as TVertex;
        }

        public bool IsBetween(Vertex<T> a, Vertex<T> b)
        {
            return a == From && b == To || a == To && b == From;
        }

        public void Unlink()
        {
            From.RemoveEdge(this);
            if (From != To) To.RemoveEdge(this);
        }

        public override string ToString() => $"{From} -- {To}";
    }

    public class WeightedEdge<T> : Edge<T>
    {
        public readonly int Weight;

        public WeightedEdge(Vertex<T> from, Vertex<T> to, int weight) : base(from, to)
        {
            Weight = weight;
        }

        public static WeightedEdge<T> Link(Vertex<T> from, Vertex<T> to, int weight) => new(from, to, weight);

        public override string ToString()
        {
            return $"{base.ToString()} [label=\" {Weight}\"];";
        }
    }

    public class DirectedEdgeOld<T> : Edge<T>
    {
        public DirectedEdgeOld(Vertex<T> from, Vertex<T> to) : base(from, to) { }

        public override Vertex<T> Other(Vertex<T> vertexOld)
        {
            return vertexOld == From ? To : null;
        }

        public override string ToString() => $"{From} -> {To}";
    }

    public class UniqueGraph<T, TVertex, TEdge> : Graph<T, TVertex, TEdge>
        where TVertex : Vertex<T>
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

        public TVertex Get(T val) => _vertices[val];

        public TVertex GetOrCreate(T val, Func<T, TVertex> cons)
        {
            if (_vertices.TryGetValue(val, out var vertex)) return vertex;
            vertex = cons(val);
            AddVertex(vertex);
            return vertex;
        }
    }
}