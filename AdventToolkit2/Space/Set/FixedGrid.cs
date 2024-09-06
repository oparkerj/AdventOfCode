using System.Numerics;
using AdventToolkit2.Collections;
using AdventToolkit2.Space.Bound;
using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space.Set;

/// <summary>
/// A grid with a fixed width and height.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class FixedGrid<TNum, T, TDim> : IGrid<TNum, T, TDim>, ISpaceView<Pos<TNum>, T, FixedGridView<TNum, T, TDim>, Rect<TNum>>, ISpacePartial<Pos<TNum>, Rect<TNum>>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>, new()
{
    public FastArray2d<T> Data;
    
    public TDim Dimension { get; set; } = new();

    public Rect<TNum> Bounds { get; }

    public int Count { get; }
    
    public T Default { get; set; } = default!;

    public FixedGrid(FastArray2d<T> data, Rect<TNum> bounds, int count)
    {
        Data = data;
        Bounds = bounds;
        Count = count;
    }
    
    public FixedGrid(TNum width, TNum height) :
        this(
            new FastArray2d<T>(int.CreateTruncating(width), int.CreateTruncating(height)),
            new Rect<TNum>(width, height),
            int.CreateTruncating(width * height))
    { }

    public FixedGrid(int width, int height) :
        this(
            new FastArray2d<T>(width, height),
            new Rect<TNum>(TNum.CreateTruncating(width), TNum.CreateTruncating(height)),
            width * height)
    { }

    public void Clear() => Data.Clear();

    public bool Contains(Pos<TNum> pos) => Bounds.Contains(pos);

    public bool Remove(Pos<TNum> pos)
    {
        if (!Contains(pos)) return false;
        this[pos] = default!;
        return true;
    }

    public IEnumerable<Pos<TNum>> Positions => Bounds;

    public IEnumerable<T> Values => Data;

    public IEnumerable<Pos<TNum>> PositionsIn(Rect<TNum> bound) => Bounds.Intersect(bound);

    public IEnumerable<Pos<TNum>> GetNeighbors(Pos<TNum> pos)
    {
        foreach (var neighbor in Dimension.GetNeighbors(pos))
        {
            if (Bounds.Contains(neighbor)) yield return neighbor;
        }
    }

    public bool ContainsValue(T val) => Data.Contains(val);

    public void Add(Pos<TNum> pos, T val) => this[pos] = val;

    public bool TryGet(Pos<TNum> pos, out T val)
    {
        if (Contains(pos))
        {
            val = Data[pos.As<int>()];
            return true;
        }

        val = default!;
        return false;
    }

    public T GetStrict(Pos<TNum> pos)
    {
        var (x, y) = pos.As<int>();
        return Data[x, y];
    }

    public T this[Pos<TNum> pos]
    {
        get
        {
            if (!Contains(pos)) return Default;
            var (x, y) = pos.As<int>();
            return Data[x, y];
        }
        set
        {
            if (!Contains(pos)) return;
            var (x, y) = pos.As<int>();
            Data[x, y] = value;
        }
    }

    public FixedGridView<TNum, T, TDim> View(Rect<TNum> bound) => new(this, bound);
}