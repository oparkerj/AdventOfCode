using System.Collections;
using System.Diagnostics;
using AdventToolkit.New.Graphing.Interface;

namespace AdventToolkit.New.Graphing.Vertices;

/// <summary>
/// Vertex model implemented as a list where all the vertices
/// are assumed to be unique.
/// In this model the vertex values are also the handles.
/// </summary>
/// <typeparam name="T"></typeparam>
public class UniqueVertexList<T> : IVertexModel<T, T>
{
    public readonly List<T> Data;

    public UniqueVertexList() : this([]) { }

    public UniqueVertexList(List<T> data) => Data = data;

    public int Count => Data.Count;

    public T Add(T t)
    {
        Debug.Assert(!Data.Contains(t));
        Data.Add(t);
        return t;
    }

    public void Remove(T t) => Data.Remove(t);

    public void RemoveHandle(T handle) => Remove(handle);

    public bool TryLookupHandle(T t, out T handle)
    {
        handle = t;
        return Contains(t);
    }

    public IEnumerable<T> LookupAll(T t)
    {
        if (TryLookupHandle(t, out var handle)) yield return handle;
    }

    public bool Contains(T t) => Data.Contains(t);

    public bool ContainsHandle(T handle) => Contains(handle);

    public void Clear() => Data.Clear();

    public List<T>.Enumerator GetEnumerator() => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}