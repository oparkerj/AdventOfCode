using System.Collections;
using AdventToolkit.New.Debugging;
using AdventToolkit.New.Graphing.Interface;

namespace AdventToolkit.New.Graphing.Vertices;

/// <summary>
/// This vertex model is backed by a list. This class supports multiple
/// vertices with the same value. Does not support removing individual vertices,
/// however the list may be cleared.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VertexIdList<T> : IVertexModel<T, int>
{
    public readonly List<T> Data;

    public VertexIdList() : this([]) { }

    public VertexIdList(List<T> data) => Data = data;

    public int Count => Data.Count;
    
    public int Add(T t)
    {
        var id = Data.Count;
        Data.Add(t);
        return id;
    }

    public void Remove(T t) => Err.NotSupported();

    public void RemoveHandle(int handle) => Err.NotSupported();

    public bool TryLookupHandle(T t, out int handle)
    {
        handle = Data.IndexOf(t);
        return handle >= 0;
    }

    public IEnumerable<int> LookupAll(T t)
    {
        for (var i = 0; i < Data.Count; i++)
        {
            if (Equals(Data[i], t)) yield return i;
        }
    }

    public bool Contains(T t) => Data.Contains(t);

    public bool ContainsHandle(int handle) => handle >= 0 && handle < Data.Count;

    public void Clear() => Data.Clear();

    public List<T>.Enumerator GetEnumerator() => Data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}