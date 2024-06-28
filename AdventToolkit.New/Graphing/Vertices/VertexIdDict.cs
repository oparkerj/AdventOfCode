using System.Collections;
using AdventToolkit.New.Graphing.Interface;

namespace AdventToolkit.New.Graphing.Vertices;

/// <summary>
/// Vertex model backed by a dictionary.
/// Removing vertices by value is slow, but quick for handles.
/// </summary>
/// <typeparam name="T"></typeparam>
public class VertexIdDict<T> : IVertexModel<T, int>
    where T : notnull
{
    public readonly Dictionary<int, T> Data;
    private int _counter;

    public VertexIdDict() : this([]) { }

    public VertexIdDict(Dictionary<int, T> data) => Data = data;

    public int Count => Data.Count;
    
    public int Add(T t)
    {
        var id = _counter++;
        Data[id] = t;
        return id;
    }

    public void Remove(T t)
    {
        foreach (var pair in Data)
        {
            if (!pair.Value.Equals(t)) continue;
            Data.Remove(pair.Key);
            break;
        }
    }

    public void RemoveHandle(int handle) => Data.Remove(handle);

    public bool TryLookupHandle(T t, out int handle)
    {
        foreach (var pair in Data)
        {
            if (!pair.Value.Equals(t)) continue;
            handle = pair.Key;
            return true;
        }
        
        handle = default;
        return false;
    }

    public IEnumerable<int> LookupAll(T t)
    {
        foreach (var pair in Data)
        {
            if (!pair.Value.Equals(t)) continue;
            yield return pair.Key;
        }
    }

    public bool Contains(T t) => Data.ContainsValue(t);

    public bool ContainsHandle(int handle) => Data.ContainsKey(handle);

    public void Clear() => Data.Clear();

    public Dictionary<int, T>.ValueCollection.Enumerator GetEnumerator() => Data.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}