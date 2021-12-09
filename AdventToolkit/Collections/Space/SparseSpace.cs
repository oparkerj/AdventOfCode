using System.Collections.Generic;

namespace AdventToolkit.Collections.Space;

public abstract class SparseSpace<TPos, TVal> : AlignedSpace<TPos, TVal>
{
    public readonly Dictionary<TPos, TVal> Points = new();

    public override int Count => Points.Count;

    public override bool TryGet(TPos pos, out TVal value)
    {
        return Points.TryGetValue(pos, out value);
    }

    public override void Add(TPos pos, TVal val)
    {
        Points[pos] = val;
    }

    public override bool Remove(TPos pos)
    {
        return Points.Remove(pos);
    }

    public override bool Has(TPos pos) => Points.ContainsKey(pos);

    public override bool HasValue(TVal val) => Points.ContainsValue(val);

    public override void Clear() => Points.Clear();

    public override IEnumerable<TPos> Positions => Points.Keys;

    public override IEnumerable<TVal> Values => Points.Values;

    public override IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator()
    {
        return Points.GetEnumerator();
    }
}