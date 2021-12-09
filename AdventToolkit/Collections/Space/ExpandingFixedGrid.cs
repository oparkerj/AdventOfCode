using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space;

public class ExpandingFixedGrid<T> : FixedGrid<T>
{
    private readonly FreeSpace<Pos, T> _extra = new();

    public ExpandingFixedGrid(int width, int height, bool includeCorners = false) : base(width, height, includeCorners)
    {
        FitBounds = true;
    }

    public ExpandingFixedGrid(FixedGrid<T> other) : base(other)
    {
        FitBounds = true;
    }

    public override int Count => base.Count + _extra.Count;

    public override bool TryGet(Pos pos, out T value)
    {
        var real = RealPosition(pos);
        if (InBounds(real)) return base.TryGet(pos, out value);
        return _extra.TryGet(pos, out value);
    }

    public override void Add(Pos pos, T val)
    {
        var real = RealPosition(pos);
        if (InBounds(real)) base.Add(pos, val);
        else _extra.Add(pos, val);
    }

    public override bool Remove(Pos pos)
    {
        var real = RealPosition(pos);
        if (InBounds(real)) return base.Remove(pos);
        return _extra.Remove(pos);
    }

    public override bool Has(Pos pos)
    {
        return base.Has(pos) || _extra.Has(pos);
    }
        
    public override bool HasValue(T val)
    {
        return base.HasValue(val) || _extra.HasValue(val);
    }

    public override void Clear()
    {
        base.Clear();
        _extra.Clear();
    }

    public override IEnumerable<Pos> Positions => base.Positions.Concat(_extra.Positions);

    public override IEnumerable<T> Values => base.Values.Concat(_extra.Values);

    public override IEnumerator<KeyValuePair<Pos, T>> GetEnumerator()
    {
        using var e = base.GetEnumerator();
        while (e.MoveNext()) yield return e.Current;
        var result = _extra.Positions.Select(pos => new KeyValuePair<Pos, T>(pos, _extra.Points[pos]));
        using var e1 = result.GetEnumerator();
        while (e.MoveNext()) yield return e1.Current;
    }
}