namespace AdventToolkit.New;

public abstract class PuzzleBase
{
    public int Part { get; init; }

    public abstract void Run();

    public virtual void WriteLn(string s) => Console.WriteLine(s);

    public virtual void WriteLn(object o) => WriteLn(o.ToString() ?? string.Empty);
}