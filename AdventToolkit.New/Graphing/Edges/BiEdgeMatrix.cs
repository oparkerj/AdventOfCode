using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Debugging;
using AdventToolkit.New.Graphing.Interface;

namespace AdventToolkit.New.Graphing.Edges;

/// <summary>
/// Edge model which uses an adjacency matrix to represent bidirectional connections.
/// This model works with integer handles, and assumes the handles have values
/// in the range [0, VertexCapacity).
/// This model supports self edges.
/// This model does not support multiple edges between the same pair of vertices.
/// </summary>
public class BiEdgeMatrix : IEdgeModel<(int, int), int>
{
    private bool[] _edges;
    private int _vertexCapacity;

    public BiEdgeMatrix() : this(0) { }

    public BiEdgeMatrix(int capacity)
    {
        _vertexCapacity = capacity;
        _edges = capacity == 0 ? [] : new bool[capacity];
    }

    private static int InternalLength(int capacity) => (capacity + 1) * (capacity / 2);

    private int Index(int from, int to)
    {
        Debug.Assert(from >= 0 && from < _vertexCapacity);
        Debug.Assert(to >= 0 && to < _vertexCapacity);
        
        if (to < from)
        {
            (from, to) = (to, from);
        }
        return (to + 1) * (to / 2) + from;
    }

    public int Count { get; private set; }

    public int VertexCapacity
    {
        get => _vertexCapacity;
        set
        {
            if (value == _vertexCapacity) return;
            Array.Resize(ref _edges, InternalLength(_vertexCapacity = value));
        }
    }

    public void EnsureVertexCapacity(int capacity) => VertexCapacity = Math.Max(VertexCapacity, capacity);

    public void Add((int, int) edge) => Link(edge.Item1, edge.Item2);

    public void Remove((int, int) edge) => Unlink(edge.Item1, edge.Item2);

    public bool Contains((int, int) edge) => IsLinked(edge.Item1, edge.Item2);

    public void Clear()
    {
        Count = 0;
        Array.Clear(_edges);
    }

    private void LinkInternal(int index)
    {
        if (_edges[index]) return;
        ++Count;
        _edges[index] = true;
    }

    public void Link(int source, int dest) => LinkInternal(Index(source, dest));

    private void UnlinkInternal(int index)
    {
        if (!_edges[index]) return;
        --Count;
        _edges[index] = false;
    }

    public void Unlink(int source, int dest) => UnlinkInternal(Index(source, dest));

    public void Unlink(int vertex)
    {
        var start = InternalLength(vertex);
        var end = start + vertex + 1;
        for (var i = start; i < end; ++i)
        {
            UnlinkInternal(i);
        }
    }

    public bool IsLinked(int source, int dest) => _edges[Index(source, dest)];

    public bool TryGetLink(int source, int dest, out (int, int) edge)
    {
        edge = (source, dest);
        return Contains(edge);
    }

    public IEnumerable<(int, int)> GetLinks(int source, int dest)
    {
        if (IsLinked(source, dest))
        {
            yield return (source, dest);
        }
    }

    public IEnumerable<(int, int)> GetLinks(int vertex)
    {
        var start = InternalLength(vertex);
        var end = start + vertex + 1;
        var delta = end - start;

        // Check for adjacency with lower values
        for (var i = start; i < end; ++i)
        {
            if (_edges[i]) yield return (end - i - 1, vertex);
        }
        // Check for adjacency with higher values
        var v = vertex + 1;
        for (var i = end + delta - 1; i < _edges.Length; v++, delta++, i += delta)
        {
            if (_edges[i]) yield return (vertex, v);
        }
    }

    public IEnumerable<(int, int)> GetIncoming(int vertex) => GetLinks(vertex);

    public IEnumerable<(int, int)> GetOutgoing(int vertex) => GetLinks(vertex);

    public Enumerator GetEnumerator() => new(_edges);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<(int, int)> IEnumerable<(int, int)>.GetEnumerator() => GetEnumerator();

    public struct Enumerator(bool[] edges) : IEnumerable<(int, int)>, IEnumerator<(int, int)>
    {
        private int _index = -1;
        
        public (int, int) Current { get; private set; } = (-1, 0);

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_index >= edges.Length || edges.Length == 0) return false;
            _index++;
            
            if (Current.Item1 < Current.Item2)
            {
                Current = Current with { Item1 = Current.Item1 + 1 };
                return true;
            }
            
            Current = (0, Current.Item2 + 1);
            return true;
        }

        public void Reset() => Err.NotSupported();

        public void Dispose() { }

        public Enumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<(int, int)> IEnumerable<(int, int)>.GetEnumerator() => GetEnumerator();
    }
}