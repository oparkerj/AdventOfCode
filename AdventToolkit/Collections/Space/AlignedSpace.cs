using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventToolkit.Collections.Space;

public abstract class AlignedSpace<TPos, TVal> : ISpace<TPos>, IEnumerable<KeyValuePair<TPos, TVal>>, IDijkstra<TPos, TPos>
{
    public bool UseDefault = true;
    public TVal Default = default;

    public abstract bool TryGet(TPos pos, out TVal value);

    public abstract void Add(TPos pos, TVal val);
        
    public abstract bool Remove(TPos pos);

    public abstract IEnumerable<TPos> GetNeighbors(TPos pos);

    public abstract int Count { get; }
        
    public virtual TVal this[TPos pos]
    {
        get => TryGet(pos, out var val) ? val : UseDefault ? Default : throw new Exception("Position doesn't exist.");
        set => Add(pos, value);
    }

    public TVal SetDefault(TPos pos, TVal val)
    {
        var has = Has(pos);
        if (!has) return this[pos] = val;
        return this[pos];
    }
        
    public virtual bool Has(TPos pos) => TryGet(pos, out _);
        
    public virtual bool HasValue(TVal val) => Values.Contains(val);
        
    public virtual void Clear()
    {
        foreach (var pos in Positions.ToList())
        {
            Remove(pos);
        }
    }

    public virtual int GetWeight(TPos mid) => 1;

    public TPos GetNeighbor(TPos from, TPos mid) => mid;

    public virtual IEnumerable<TPos> Positions => this.Keys();

    public virtual IEnumerable<TVal> Values => this.Values();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public abstract IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator();
}

public class CopySpace<TPos, TVal> : AlignedSpace<TPos, TVal>
{
    private readonly AlignedSpace<TPos, TVal> _source;

    public CopySpace(AlignedSpace<TPos, TVal> reference)
    {
        _source = reference;
    }

    public override IEnumerable<TPos> GetNeighbors(TPos pos) => _source.GetNeighbors(pos);

    public override bool TryGet(TPos pos, out TVal value)
    {
        return _source.TryGet(pos, out value);
    }

    public override void Add(TPos pos, TVal val)
    {
        _source.Add(pos, val);
    }

    public override bool Remove(TPos pos)
    {
        return _source.Remove(pos);
    }

    public override int Count => _source.Count;
        
    public override IEnumerator<KeyValuePair<TPos, TVal>> GetEnumerator()
    {
        return _source.GetEnumerator();
    }
}

public class FreeSpace<TPos, TVal> : SparseSpace<TPos, TVal>
{
    public override IEnumerable<TPos> GetNeighbors(TPos pos)
    {
        throw new NotSupportedException();
    }
}