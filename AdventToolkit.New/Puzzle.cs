namespace AdventToolkit.New;

/// <summary>
/// Puzzle base class where the two parts return different types.
/// </summary>
/// <typeparam name="T1">Part one type.</typeparam>
/// <typeparam name="T2">Part two type.</typeparam>
public abstract class Puzzle<T1, T2> : PuzzleBase
    where T1 : notnull
    where T2 : notnull
{
    private string[]? _lines;
    
    public Puzzle() => Part = 2;

    /// <summary>
    /// Each line of the puzzle input.
    /// </summary>
    public string[] Input
    {
        get => _lines ??= GetLines();
        set => _lines = value;
    }

    /// <summary>
    /// Run the first part of the puzzle
    /// </summary>
    /// <returns>Part one solution.</returns>
    public abstract T1 PartOne();

    /// <summary>
    /// Run the second part of the puzzle.
    /// </summary>
    /// <returns>Part two solution.</returns>
    public abstract T2 PartTwo();
    
    /// <summary>
    /// Runs part one if Part = 1, otherwise part two.
    /// </summary>
    public override void Run()
    {
        if (Part == 1) WriteLn(PartOne());
        else WriteLn(PartTwo());
    }

    /// <summary>
    /// Get the input name for a given type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string InputName(Type type) => type.Name + ".txt";

    /// <summary>
    /// Get the input name for a given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string InputName<T>() => InputName(typeof(T));

    /// <summary>
    /// Get the puzzles input name.
    /// By default this is the <see cref="InputName(System.Type)"/> of the current type.
    /// </summary>
    /// <returns></returns>
    public virtual string InputName() => InputName(GetType());

    public string InputDirectory { get; set; } = string.Empty;
    
    public override string GetInput()
    {
        var path = Path.Combine(InputDirectory, InputName());
        Console.WriteLine($"Reading {path}");
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Split the input into lines.
    /// </summary>
    /// <returns></returns>
    public string[] GetLines()
    {
        var lines = new List<string>();
        foreach (var line in RawInput.AsSpan().EnumerateLines())
        {
            lines.Add(line.ToString());
        }
        return lines.ToArray();
    }
}

/// <summary>
/// Puzzle base class where the two parts return the same type.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Puzzle<T> : Puzzle<T, T>
    where T : notnull
{
    public override T PartTwo() => PartOne();
}