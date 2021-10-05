using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventToolkit.Utilities
{
    public class Graph<T, TVertex, TEdge> : IEnumerable<TVertex>
        where TVertex : Vertex<T>
        where TEdge : Edge<T>
    {
        private int _counter;
        
        internal readonly Dictionary<int, TVertex> _vertices = new();

        public int Count => _vertices.Count;

        public Vertex<T> this[int id] => _vertices[id];

        public virtual void AddVertex(TVertex vertex)
        {
            vertex.Id = _counter++;
            _vertices[vertex.Id] = vertex;
        }

        public virtual void RemoveVertex(TVertex vertex)
        {
            if (_vertices.TryGetValue(vertex.Id, out var v) && v != vertex) return;
            foreach (var edge in vertex._edges.ToList())
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
            if (typeof(TEdge) == typeof(DirectedEdge<T>)) b.Append("digraph G {\n");
            else b.Append("graph G {\n");
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
    
    public class Graph<T> : Graph<T, Vertex<T>, Edge<T>> { }

    public class Vertex<T>
    {
        public int Id { get; internal set; }
        
        public T Value;
        
        internal readonly List<Edge<T>> _edges = new();
        
        public Vertex(T value)
        {
            Value = value;
        }

        public int Count => _edges.Count;
        
        public IEnumerable<Edge<T>> Edges => _edges;

        public IEnumerable<Vertex<T>> Neighbors => Edges.Select(edge => edge.Other(this)).Where(v => v != null);

        internal void RemoveEdge(int index)
        {
            _edges.RemoveAt(index);
            for (var i = index; i < _edges.Count; i++)
            {
                _edges[i].Decrement(this);
            }
        }

        public bool ConnectedTo(Vertex<T> other)
        {
            return _edges.Any(edge => edge.IsBetween(this, other));
        }
        
        public override string ToString() => $"\"{Value}\"";
    }

    public class Edge<T>
    {
        public Vertex<T> From { get; private set; }
        public Vertex<T> To { get; private set; }
        private int _fromIndex;
        private int _toIndex;

        public Edge(Vertex<T> from, Vertex<T> to)
        {
            From = from;
            To = to;
            _fromIndex = from._edges.Count;
            from._edges.Add(this);
            _toIndex = to._edges.Count;
            if (From != To) to._edges.Add(this);
        }

        public static void Connect(Vertex<T> from, Vertex<T> to)
        {
            _ = new Edge<T>(from, to);
        }

        internal void Decrement(Vertex<T> vertex)
        {
            if (vertex == From) _fromIndex--;
            if (vertex == To) _toIndex--;
        }

        public void Unlink()
        {
            From.RemoveEdge(_fromIndex);
            if (From != To) To.RemoveEdge(_toIndex);
            From = null;
            To = null;
        }

        public bool IsBetween(Vertex<T> a, Vertex<T> b)
        {
            return From == a && To == b || From == b && To == a;
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

        public override string ToString() => $"{From} -- {To}";
    }

    public class WeightedEdge<T> : Edge<T>
    {
        public int Weight;

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

    public class DirectedEdge<T> : Edge<T>
    {
        public DirectedEdge(Vertex<T> from, Vertex<T> to) : base(from, to) { }

        public override Vertex<T> Other(Vertex<T> vertex)
        {
            return vertex == From ? To : null;
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
            _vertices[val] = vertex;
            return vertex;
        }
    }
}