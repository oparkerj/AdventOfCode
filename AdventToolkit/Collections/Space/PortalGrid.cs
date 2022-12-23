using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Space;

// Grid where portals may be added between spaces on certain sides
public class PortalGrid<T> : Grid<T>
{
    private Dictionary<Pos, Dictionary<Side, Pos>> _portals = new();

    public PortalGrid() { }

    public static PortalGrid<T> CreateFrom(PortalGrid<T> grid)
    {
        var g = new PortalGrid<T>();
        foreach (var (key, value) in grid._portals)
        {
            g._portals[key] = value;
        }
        return g;
    }

    private Dictionary<Side, Pos> GetPortals(Pos p)
    {
        return _portals.GetOrSetValue(p, () => new Dictionary<Side, Pos>(1));
    }

    private IEnumerable<Side> Sides()
    {
        yield return Side.Top;
        yield return Side.Right;
        yield return Side.Bottom;
        yield return Side.Left;
    }

    public override IEnumerable<Pos> GetNeighbors(Pos pos)
    {
        if (!_portals.TryGetValue(pos, out var sides)) return base.GetNeighbors(pos);
        return Sides().Except(sides.Keys).Select(side => pos.GetSide(side)).Concat(sides.Values);
    }

    public Pos GetNeighbor(Pos pos, Side side)
    {
        if (!_portals.TryGetValue(pos, out var sides)) return pos.GetSide(side);
        return sides.TryGetValue(side, out var result) ? result : pos.GetSide(side);
    }

    public T GetNeighborValue(Pos pos, Side side)
    {
        return this[GetNeighbor(pos, side)];
    }
        
    public void Connect(Pos a, Side aSide, Pos b, Side bSide)
    {
        var aPortals = GetPortals(a);
        var bPortals = GetPortals(b);
        aPortals[aSide] = b;
        bPortals[bSide] = a;
    }

    public void WrapEdges(Rect rect)
    {
        foreach (var (top, bottom) in rect.GetSidePositions(Side.Top).Zip(rect.GetSidePositions(Side.Bottom)))
        {
            Connect(top, Side.Top, bottom, Side.Bottom);
        }
        foreach (var (left, right) in rect.GetSidePositions(Side.Left).Zip(rect.GetSidePositions(Side.Right)))
        {
            Connect(left, Side.Left, right, Side.Right);
        }
    }
}

// Portal grid, except positions have an associated tag.
// This can be used to keep track of traversed portals.
// Must define a pass function that determines if you are allowed
// to pass through the portal with the given tag, and what your new
// modified tag would be.
public class PortalGrid<T, TTag> : SparseSpace<(Pos Pos, TTag Tag), T>
{
    private Dictionary<Pos, Dictionary<Side, (Pos Pos, TTag Tag)>> _portals = new();
    public PortalPass PassFunction;

    public delegate bool PortalPass((Pos Pos, TTag Tag) current, Pos portal, ref TTag tag);

    public T this[Pos pos]
    {
        get => base[(pos, default)];
        set => base[(pos, default)] = value;
    }

    public override T this[(Pos Pos, TTag Tag) pos]
    {
        get => this[pos.Pos];
        set => this[pos.Pos] = value;
    }

    private Dictionary<Side, (Pos Pos, TTag Tag)> GetPortals(Pos p)
    {
        return _portals.GetOrSetValue(p, () => new Dictionary<Side, (Pos Pos, TTag Tag)>(1));
    }

    private IEnumerable<Side> Sides()
    {
        yield return Side.Top;
        yield return Side.Right;
        yield return Side.Bottom;
        yield return Side.Left;
    }

    public bool Has(Pos pos)
    {
        return Positions.Any(tuple => tuple.Pos == pos);
    }

    public T GetAny(Pos pos) => this[Positions.First(tuple => tuple.Pos == pos)];

    public (Pos Pos, TTag Tag) GetNeighborSide(Pos pos, Side side)
    {
        if (!_portals.TryGetValue(pos, out var sides)) return (pos.GetSide(side), default);
        if (sides.TryGetValue(side, out var otherPos)) return otherPos;
        return (pos.GetSide(side), default);
    }

    public (Pos Pos, TTag Tag) GetNeighborSide((Pos Pos, TTag Tag) pos, Side side)
    {
        return GetNeighborSide(pos.Pos, side);
    }

    public override IEnumerable<(Pos Pos, TTag Tag)> GetNeighbors((Pos Pos, TTag Tag) pos)
    {
        if (!_portals.TryGetValue(pos.Pos, out var sides)) return pos.Pos.Adjacent().Select(p => (p, pos.Tag));
        return Sides().Except(sides.Keys).Select(side => (pos.Pos.GetSide(side), pos.Tag))
            .Concat(sides.Values.SelectWhere(((Pos Pos, TTag Tag) input, out (Pos, TTag Tag) output) =>
            {
                var (p, tag) = input;
                if (!PassFunction(pos, p, ref tag))
                {
                    output = default;
                    return false;
                }
                output = (p, tag);
                return true;
            }));
    }

    public void Connect(Pos a, Side aSide, TTag aTag, Pos b, Side bSide, TTag bTag)
    {
        var aPortals = GetPortals(a);
        var bPortals = GetPortals(b);
        aPortals[aSide] = (b, aTag);
        bPortals[bSide] = (a, bTag);
    }
}