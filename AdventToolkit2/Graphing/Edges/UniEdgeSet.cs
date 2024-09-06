using System.Collections;
using AdventToolkit2.Debugging;
using AdventToolkit2.Graphing.Interface;

namespace AdventToolkit2.Graphing.Edges;

public class UniEdgeSet<T> : IEdgeModel<(T, T), T>
{
    public readonly HashSet<(T, T)> Edges;

    public UniEdgeSet() : this([]) { }

    public UniEdgeSet(HashSet<(T, T)> edges) => Edges = edges;

    public int Count => Edges.Count;
    
    public int VertexCapacity { get; set; }

    public void EnsureVertexCapacity(int capacity)
    {
        // This is an estimate that can be adjusted as needed.
        // For now, estimate twice the number of edges as vertices.
        Edges.EnsureCapacity(capacity * 2);
    }

    public void Clear() => Edges.Clear();

    public void Add((T, T) edge) => Edges.Add(edge);

    public void Remove((T, T) edge) => Edges.Remove(edge);

    public bool Contains((T, T) edge) => Edges.Contains(edge);

    public void Link(T source, T dest) => Add((source, dest));

    public void Unlink(T source, T dest) => Remove((source, dest));

    public void Unlink(T vertex)
    {
        // Looping manually would be faster than the delegate, however the builtin function
        // is able to remove the items in one pass, which just barely makes it faster.
        Edges.RemoveWhere(tuple => Equals(tuple.Item1, vertex) || Equals(tuple.Item2, vertex));
    }

    public bool IsLinked(T source, T dest) => Edges.Contains((source, dest));

    public bool TryGetLink(T source, T dest, out (T, T) edge)
    {
        edge = (source, dest);
        return Contains(edge);
    }

    public IEnumerable<(T, T)> GetLinks(T source, T dest)
    {
        if (IsLinked(source, dest)) yield return (source, dest);
    }

    public BiEdgeSet<T>.Enumerator GetLinks(T vertex) => new(vertex, Edges.GetEnumerator());

    IEnumerable<(T, T)> IEdgeModel<(T, T), T>.GetLinks(T vertex) => GetLinks(vertex);

    public IncomingEnumerator GetIncoming(T vertex) => new(vertex, Edges.GetEnumerator());

    IEnumerable<(T, T)> IEdgeModel<(T, T), T>.GetIncoming(T vertex) => GetIncoming(vertex);

    public OutgoingEnumerator GetOutgoing(T vertex) => new(vertex, Edges.GetEnumerator());

    IEnumerable<(T, T)> IEdgeModel<(T, T), T>.GetOutgoing(T vertex) => GetOutgoing(vertex);

    public HashSet<(T, T)>.Enumerator GetEnumerator() => Edges.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<(T, T)> IEnumerable<(T, T)>.GetEnumerator() => GetEnumerator();

    public struct IncomingEnumerator(T value, HashSet<(T, T)>.Enumerator enumerator) : IEnumerable<(T, T)>, IEnumerator<(T, T)>
    {
        public (T, T) Current { get; private set; }
        
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (enumerator.MoveNext())
            {
                if (!Equals(enumerator.Current.Item2, value)) continue;
                Current = enumerator.Current;
                return true;
            }
            return false;
        }

        public void Reset() => Err.NotSupported();

        public void Dispose() { }

        public IncomingEnumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<(T, T)> IEnumerable<(T, T)>.GetEnumerator() => GetEnumerator();
    }
    
    public struct OutgoingEnumerator(T value, HashSet<(T, T)>.Enumerator enumerator) : IEnumerable<(T, T)>, IEnumerator<(T, T)>
    {
        public (T, T) Current { get; private set; }
        
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (enumerator.MoveNext())
            {
                if (!Equals(enumerator.Current.Item1, value)) continue;
                Current = enumerator.Current;
                return true;
            }
            return false;
        }

        public void Reset() => Err.NotSupported();

        public void Dispose() { }

        public OutgoingEnumerator GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<(T, T)> IEnumerable<(T, T)>.GetEnumerator() => GetEnumerator();
    }
}