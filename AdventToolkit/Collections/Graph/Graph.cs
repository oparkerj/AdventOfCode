using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventToolkit.Collections.Graph
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
}