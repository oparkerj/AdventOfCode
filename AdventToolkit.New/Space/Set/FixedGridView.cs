using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using AdventToolkit.New.Space.Bound;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// A view of a fixed grid.
///
/// This can operate on a limited area of the original grid
/// at the cost of some performance.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class FixedGridView<TNum, T, TDim> : IGrid<TNum, T, TDim>, ISpaceView<Pos<TNum>, T, FixedGridView<TNum, T, TDim>, Rect<TNum>>, ISpacePartial<Pos<TNum>, Rect<TNum>>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>
{
    public FixedGrid<TNum, T, TDim> Source;
    
    private Rect<TNum> _bounds;

    /// <summary>
    /// Effective area of the view.
    /// </summary>
    public Rect<TNum> ViewBounds { get; private set; }

    public int Count { get; private set; }

    public T Default { get; set; } = default!;

    public FixedGridView(FixedGrid<TNum, T, TDim> source, Rect<TNum> bounds)
    {
        Source = source;
        Bounds = bounds;
    }
    
    public Rect<TNum> Bounds
    {
        get => _bounds;
        
        [MemberNotNull(nameof(_bounds), nameof(ViewBounds))]
        set
        {
            _bounds = value;
            ViewBounds = Source.Bounds.Intersect(value);
            Count = int.CreateTruncating(ViewBounds.Area());
        }
    }

    public IEnumerable<Pos<TNum>> Positions => ViewBounds;

    public IEnumerable<T> Values => Source.Data[ViewBounds.As<int>()];

    public IEnumerable<Pos<TNum>> PositionsIn(Rect<TNum> bound) => ViewBounds.Intersect(bound);

    public IEnumerable<Pos<TNum>> GetNeighbors(Pos<TNum> pos)
    {
        foreach (var neighbor in TDim.GetNeighbors(pos))
        {
            if (ViewBounds.Contains(neighbor)) yield return neighbor;
        }
    }

    public void Clear() => Source.Data[ViewBounds.As<int>()].Clear();

    public bool Remove(Pos<TNum> pos)
    {
        if (!Contains(pos)) return false;
        this[pos] = default!;
        return true;
    }

    public void Add(Pos<TNum> pos, T val) => this[pos] = val;

    public bool Contains(Pos<TNum> pos) => ViewBounds.Contains(pos);

    public bool ContainsValue(T val) => Source.Data[ViewBounds.As<int>()].Contains(val);

    public bool TryGet(Pos<TNum> pos, out T val)
    {
        if (Contains(pos))
        {
            val = Source.Data[pos.As<int>()];
            return true;
        }

        val = default!;
        return false;
    }

    public T GetStrict(Pos<TNum> pos) => Source.Data[pos.As<int>()];

    public T this[Pos<TNum> pos]
    {
        get => !Contains(pos) ? Default : Source.Data[pos.As<int>()];
        set
        {
            if (!Contains(pos)) return;
            Source.Data[pos.As<int>()] = value;
        }
    }

    public FixedGridView<TNum, T, TDim> View(Rect<TNum> bound) => new(Source, bound);
}