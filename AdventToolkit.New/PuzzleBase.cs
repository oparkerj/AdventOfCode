namespace AdventToolkit.New;

public abstract class PuzzleBase
{
    public int Part { get; init; }

    private string? _input;

    public string Input
    {
        get => _input ??= GetInput();
        set => _input = value;
    }

    public abstract void Run();

    public virtual string GetInput() => File.ReadAllText(GetType().Name + ".txt");

    public virtual void WriteLn(string s) => Console.WriteLine(s);

    public virtual void WriteLn(object o) => WriteLn(o.ToString() ?? string.Empty);
}