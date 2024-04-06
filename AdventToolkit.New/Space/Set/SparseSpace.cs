using System.Collections;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

public abstract class SparseSpace<TPos, TVal> : ISpace<TPos, TVal>, IEnumerable<KeyValuePair<TPos, TVal>>
    where TPos : notnull
{
    /// <summary>
    /// Internal position mapping.
    /// </summary>
    public readonly Dictionary<TPos, TVal> Points = new();

    public int Count => Points.Count;

    public TVal Default { get; set; } = default!;

    public void Add(TPos pos, TVal val) => Points[pos] = val;

    public bool Remove(TPos pos) => Points.Remove(pos);

    public TVal Get(TPos pos) => Points.GetValueOrDefault(pos, Default);

    public TVal GetStrict(TPos pos) => Points[pos];

    public bool TryGet(TPos pos, out TVal val) => Points.TryGetValue(pos, out val!);

    public TVal this[TPos pos]
    {
        get => Points.GetValueOrDefault(pos, Default);
        set => Points[pos] = value;
    }

    public bool Contains(TPos pos) => Points.ContainsKey(pos);

    public bool ContainsValue(TVal val) => Points.ContainsValue(val);

    public void Clear() => Points.Clear();

    public abstract IEnumerable<TPos> GetNeighbors(TPos pos);

    public IEnumerable<TPos> Positions => Points.Keys;

    public IEnumerable<TVal> Values => Points.Values;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator() => Points.GetEnumerator();
}