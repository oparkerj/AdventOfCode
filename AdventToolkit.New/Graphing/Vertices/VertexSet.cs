using System.Collections;
using AdventToolkit.New.Graphing.Interface;

namespace AdventToolkit.New.Graphing.Vertices;

/// <summary>
/// Vertex model implemented using a set.
/// In this model the vertex values are also the handles.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VertexSet<T> : IVertexModel<T, T>
{
    public readonly HashSet<T> Data;
    
    public VertexSet() : this([]) { }

    public VertexSet(HashSet<T> data) => Data = data;

    public int Count => Data.Count;
    
    public T Add(T t)
    {
        Data.Add(t);
        return t;
    }

    public void Remove(T t) => Data.Remove(t);

    public void RemoveHandle(T handle) => Remove(handle);

    public bool TryLookupHandle(T t, out T handle)
    {
        if (Data.Contains(t))
        {
            handle = t;
            return true;
        }

        handle = default!;
        return false;
    }

    public IEnumerable<T> LookupAll(T t)
    {
        if (Data.Contains(t)) yield return t;
    }

    public bool Contains(T t) => Data.Contains(t);

    public bool ContainsHandle(T handle) => Contains(handle);

    public void Clear() => Data.Clear();

    public HashSet<T>.Enumerator GetEnumerator() => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}