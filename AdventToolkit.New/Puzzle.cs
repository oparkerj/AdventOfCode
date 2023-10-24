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
    public Puzzle() => Part = 2;

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