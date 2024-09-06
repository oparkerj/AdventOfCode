using System.Collections;
using AdventToolkit2.Collections;
using AdventToolkit2.Debugging;
using AdventToolkit2.Graphing.Interface;

namespace AdventToolkit2.Graphing.Edges;

/// <summary>
/// Edge model which uses an adjacency matrix to represent unidirectional connections.
/// This model works with integer handles, and assumes the handles have values
/// in the range [0, VertexCapacity).
/// This model supports self edges.
/// This model does not support multiple edges between the same pair of vertices.
/// </summary>
public class UniEdgeMatrix : IEdgeModel<(int, int), int>
{
    public FastArray2d<bool> _edges;
    public int _vertexCapacity;

    public UniEdgeMatrix() : this(0) { }

    public UniEdgeMatrix(int capacity)
    {
        _vertexCapacity = capacity;
        _edges = new FastArray2d<bool>(capacity, capacity);
    }

    public int Count { get; private set; }

    public int VertexCapacity
    {
        get => _vertexCapacity;
        set
        {
            if (value == _vertexCapacity) return;
            var len = _vertexCapacity;
            _vertexCapacity = value;
            var prev = _edges;
            var next = new FastArray2d<bool>(value, value);
            for (var i = 0; i < len; i++)
            {
                for (var j = 0; j < len; j++)
                {
                    next[i, j] = prev[i, j];
                }
            }
            _edges = next;
        }
    }

    public void EnsureVertexCapacity(int capacity) => VertexCapacity = Math.Max(VertexCapacity, capacity);

    public void Clear()
    {
        Count = 0;
        _edges.Clear();
    }

    public void Add((int, int) edge) => Link(edge.Item1, edge.Item2);

    public void Remove((int, int) edge) => Unlink(edge.Item1, edge.Item2);

    public bool Contains((int, int) edge) => IsLinked(edge.Item1, edge.Item2);

    public void Link(int source, int dest)
    {
        if (_edges[source, dest]) return;
        ++Count;
        _edges[source, dest] = true;
    }

    public void Unlink(int source, int dest)
    {
        if (!_edges[source, dest]) return;
        --Count;
        _edges[source, dest] = false;
    }

    public void Unlink(int vertex)
    {
        var data = _edges.Data;
        foreach (var i in _edges.Row(vertex).GetIndexEnumerator())
        {
            if (!data[i]) continue;
            --Count;
            data[i] = false;
        }
        foreach (var i in _edges.Col(vertex).GetIndexEnumerator())
        {
            if (!data[i]) continue;
            --Count;
            data[i] = false;
        }
    }

    public bool IsLinked(int source, int dest) => _edges[source, dest];

    public bool TryGetLink(int source, int dest, out (int, int) edge)
    {
        edge = (source, dest);
        return Contains(edge);
    }

    public IEnumerable<(int, int)> GetLinks(int vertex)
    {
        var data = _edges.Data;
        var other = 0;
        foreach (var i in _edges.Row(vertex).GetIndexEnumerator())
        {
            if (data[i]) yield return (other, vertex);
            other++;
        }
        other = 0;
        foreach (var i in _edges.Col(vertex).GetIndexEnumerator())
        {
            if (other == vertex) continue;
            if (data[i]) yield return (vertex, other);
            other++;
        }
    }

    public IEnumerable<(int, int)> GetIncoming(int vertex)
    {
        var data = _edges.Data;
        var other = 0;
        foreach (var i in _edges.Row(vertex).GetIndexEnumerator())
        {
            if (data[i]) yield return (other, vertex);
            other++;
        }
    }

    public IEnumerable<(int, int)> GetOutgoing(int vertex)
    {
        var data = _edges.Data;
        var other = 0;
        foreach (var i in _edges.Col(vertex).GetIndexEnumerator())
        {
            if (data[i]) yield return (vertex, other);
            other++;
        }
    }

    public IEnumerable<(int, int)> GetLinks(int source, int dest)
    {
        if (IsLinked(source, dest)) yield return (source, dest);
    }

    public Enumerator GetEnumerator() => new(_edges.Data, _vertexCapacity);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<(int, int)> IEnumerable<(int, int)>.GetEnumerator() => GetEnumerator();

    public struct Enumerator(bool[] edges, int size) : IEnumerable<(int, int)>, IEnumerator<(int, int)>
    {
        private int _index;

        public (int, int) Current { get; private set; } = (-1, 0);
        
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (_index < edges.Length)
            {
                if (Current.Item1 + 1 >= size)
                {
                    Current = (0, Current.Item2 + 1);
                }
                else
                {
                    Current = Current with { Item1 = Current.Item1 + 1 };
                }
                
                if (!edges[_index++]) continue;
                return true;
            }
            
            return false;
        }

        public void Reset() => Err.NotSupported();

        public void Dispose() { }

        public Enumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<(int, int)> IEnumerable<(int, int)>.GetEnumerator() => GetEnumerator();
    }
}