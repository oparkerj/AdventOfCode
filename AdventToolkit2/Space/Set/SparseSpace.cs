using System.Collections;
using System.Diagnostics.CodeAnalysis;
using AdventToolkit2.Space.Bound;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Set;

/// <summary>
/// Base class for a space where mappings are stored sparsely.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
/// <typeparam name="TBound"></typeparam>
public class SparseSpace<TPos, TVal, TBound> :
    ISpace<TPos, TVal>,
    IBounded<TPos, TBound>,
    IDictionary<TPos, TVal>
    where TPos : notnull
    where TBound : IBound<TBound, TPos>
{
    /// <summary>
    /// Internal position mapping.
    /// </summary>
    public readonly Dictionary<TPos, TVal> Points = new();

    public int Count => Points.Count;

    public TVal Default { get; set; } = default!;

    public TBound Bounds { get; set; } = TBound.Empty;

    public void Add(TPos pos, TVal val)
    {
        Bounds = Bounds.Add(pos);
        Points[pos] = val;
    }

    public bool Remove(TPos pos) => Points.Remove(pos);

    public TVal Get(TPos pos) => Points.GetValueOrDefault(pos, Default);

    public TVal GetStrict(TPos pos) => Points[pos];

    public bool TryGet(TPos pos, out TVal val) => Points.TryGetValue(pos, out val!);

    public TVal this[TPos pos]
    {
        get => Points[pos];
        set
        {
            Bounds = Bounds.Add(pos);
            Points[pos] = value;
        }
    }

    public bool Contains(TPos pos) => Points.ContainsKey(pos);

    public bool ContainsValue(TVal val) => Points.ContainsValue(val);

    public void Clear() => Points.Clear();

    public IEnumerable<TPos> Positions => Points.Keys;

    public IEnumerable<TVal> Values => Points.Values;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator() => Points.GetEnumerator();

    #region Dictionary Methods

    private ICollection<KeyValuePair<TPos, TVal>> Collection => Points;
    
    public void Add(KeyValuePair<TPos, TVal> item) => this[item.Key] = item.Value;

    public bool Contains(KeyValuePair<TPos, TVal> item) => Collection.Contains(item);

    public void CopyTo(KeyValuePair<TPos, TVal>[] array, int arrayIndex) => Collection.CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TPos, TVal> item) => Collection.Remove(item);

    public bool IsReadOnly => false;
    
    public bool ContainsKey(TPos key) => Contains(key);
    
    public bool TryGetValue(TPos key, [MaybeNullWhen(false)] out TVal value) => Points.TryGetValue(key, out value);

    public ICollection<TPos> Keys => Points.Keys;

    ICollection<TVal> IDictionary<TPos, TVal>.Values => Points.Values;

    #endregion
}

/// <summary>
/// Defines an unbounded space.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
public class SparseSpace<TPos, TVal> : SparseSpace<TPos, TVal, Unbounded<TPos>>
    where TPos : notnull;