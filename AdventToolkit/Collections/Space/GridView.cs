using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space;

// Provide a window into a grid, which may be transformed.
// You interact with the grid through the original window, and values are
// read/written at transformed locations.
// This applies transforms in the order flips, offset.
public class GridView<T> : GridBase<T>
{
    public readonly GridBase<T> Source;
    
    public bool FlipH { get; set; }
    public bool FlipV { get; set; }
    public bool Transpose { get; set; }
    public Pos Offset { get; set; }

    public int OffsetX
    {
        get => Offset.X;
        set => Offset = new Pos(value, Offset.Y);
    }

    public int OffsetY
    {
        get => Offset.Y;
        set => Offset = new Pos(Offset.X, value);
    }

    private GridView() => FitBounds = false;

    public GridView(GridBase<T> source) : this()
    {
        Source = source;
        Bounds = source.Bounds;
    }

    public GridView(GridBase<T> source, Rect window) : this()
    {
        Source = source;
        Bounds = window;
    }

    public Pos RealPos(Pos windowPos)
    {
        var (x, y) = windowPos;
        if (Transpose) (x, y) = (y, x);
        var bounds = Bounds;
        if (FlipH) x = (int) (x + (bounds.MidX - x) * 2);
        if (FlipV) y = (int) (y + (bounds.MidY - y) * 2);
        return new Pos(x, y) + Offset;
    }

    public Pos Reverse(Pos realPos)
    {
        var (x, y) = realPos - Offset;
        var bounds = Bounds;
        if (FlipH) x = (int) (x + (bounds.MidX - x) * 2);
        if (FlipV) y = (int) (y + (bounds.MidY - y) * 2);
        if (Transpose) (x, y) = (y, x);
        return new Pos(x, y);
    }

    public override bool TryGet(Pos pos, out T value)
    {
        return Source.TryGet(RealPos(pos), out value);
    }

    public override void Add(Pos pos, T val)
    {
        Source.Add(RealPos(pos), val);
    }

    public override bool Remove(Pos pos)
    {
        return Source.Remove(RealPos(pos));
    }

    public override int Count => Source.Positions.Count(Bounds.Contains);

    public void OverlayTransformed(Func<T, T, T> combine)
    {
        var target = new Rect(Bounds);
        if (Transpose) (target.Width, target.Height) = (target.Height, target.Width);
        (target.MinXFixed, target.MinYFixed) = target.Min + Offset;
        foreach (var pos in Source.Positions.Where(target.Contains).ToList())
        {
            var windowPos = Reverse(pos);
            Source[windowPos] = combine(Source[windowPos], Source[pos]);
        }
    }

    public override IEnumerator<KeyValuePair<Pos, T>> GetEnumerator()
    {
        return Source.Positions.Where(Bounds.Contains)
            .Select(pos => new KeyValuePair<Pos, T>(pos, Source[RealPos(pos)])).GetEnumerator();
    }

    public IEnumerable<KeyValuePair<Pos, T>> AdjustedPositions()
    {
        return Source.Positions.Where(Bounds.Contains)
            .Select(RealPos)
            .Select(pos => new KeyValuePair<Pos, T>(pos, Source[pos]));
    }
}