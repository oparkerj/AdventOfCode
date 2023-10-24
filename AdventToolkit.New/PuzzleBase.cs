namespace AdventToolkit.New;

/// <summary>
/// Base class to create puzzles.
/// </summary>
public abstract class PuzzleBase
{
    /// <summary>
    /// Which part of the puzzle to execute.
    /// </summary>
    public int Part { get; init; }

    private string? _input;

    /// <summary>
    /// Get the puzzle input. If unset, calls <see cref="GetInput"/> and
    /// caches the value.
    /// </summary>
    public string Input
    {
        get => _input ??= GetInput();
        set => _input = value;
    }

    /// <summary>
    /// Execute the puzzle.
    /// </summary>
    public abstract void Run();

    /// <summary>
    /// Get the input for the puzzle.
    /// By default, reads a text file with the same name as the
    /// implementing class.
    /// </summary>
    /// <returns></returns>
    public virtual string GetInput() => File.ReadAllText(GetType().Name + ".txt");

    /// <summary>
    /// Print some output during puzzle execution.
    /// </summary>
    /// <param name="s"></param>
    public virtual void WriteLn(string s) => Console.WriteLine(s);

    /// <inheritdoc cref="WriteLn(string)"/>
    public virtual void WriteLn(object o) => WriteLn(o.ToString() ?? string.Empty);
}