using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Common;

namespace AdventToolkit.Collections.Space;

public class GridWindow<T> : GridBase<T>
{
    public readonly GridBase<T> Source;
    
    public bool FlipH { get; set; }
    public bool FlipV { get; set; }
    public bool Transpose { get; set; }
    // TODO

    private GridWindow() => FitBounds = false;

    public GridWindow(GridBase<T> source) : this()
    {
        Source = source;
        Bounds = source.Bounds;
    }

    public GridWindow(GridBase<T> source, Rect window) : this()
    {
        Source = source;
        Bounds = window;
    }

    public Pos RealPos(Pos windowPos) => windowPos;

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
    
    public override IEnumerator<KeyValuePair<Pos, T>> GetEnumerator()
    {
        return Bounds.Select(pos => new KeyValuePair<Pos, T>(pos, Source[RealPos(pos)])).GetEnumerator();
    }
}