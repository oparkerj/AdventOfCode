namespace AdventToolkit.New.Data;

public record Rect(Interval Horizontal, Interval Vertical)
{
    public Rect(Pos a, Pos b) : this(Interval.Span(a.X, b.X), Interval.Span(a.Y, b.Y)) { }
    
    public int Width => Horizontal.Length;

    public int Height => Vertical.Length;

    public int MinX => Horizontal.Start;

    public int MinY => Vertical.Start;

    public int MaxX => Horizontal.Last;

    public int MaxY => Vertical.Last;

    public int EndX => Horizontal.End;

    public int EndY => Vertical.End;

    public Pos Min => new(MinX, MinY);

    public Pos Max => new(MaxX, MaxY);

    public Pos Dimensions => new(Width, Height);

    public bool Contains(Pos p) => Horizontal.Contains(p.X) && Vertical.Contains(p.Y);

    public Rect Intersect(Rect other) => new(Horizontal.Intersect(other.Horizontal), Vertical.Intersect(other.Vertical));
}