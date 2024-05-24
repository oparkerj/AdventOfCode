using System.Numerics;
using AdventToolkit.New.Collections;
using AdventToolkit.New.Space.Bound;
using AdventToolkit.New.Space.Interface;

namespace AdventToolkit.New.Space.Set;

/// <summary>
/// A grid with a fixed width and height.
/// </summary>
/// <typeparam name="TNum"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TDim"></typeparam>
public class FixedGrid<TNum, T, TDim> : IGrid<TNum, T, TDim>
    where TNum : INumber<TNum>
    where TDim : IDimension<Pos<TNum>>
{
    public FastArray2d<T> Data;
    
    public Rect<TNum> Bounds { get; }

    public T Default { get; set; } = default!;
    
    public int Count { get; }

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
    
    public bool ContainsValue(T val) => Data.Contains(val);

    public void Add(Pos<TNum> pos, T val) => this[pos] = val;

    public bool TryGet(Pos<TNum> pos, out T val)
    {
        if (Contains(pos))
        {
            var (x, y) = pos.As<int>();
            val = Data[x, y];
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
}