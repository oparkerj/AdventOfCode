namespace AdventToolkit.New;

public abstract class Puzzle<T1, T2> : PuzzleBase
    where T1 : notnull
    where T2 : notnull
{
    public Puzzle() => Part = 2;

    public abstract T1 PartOne();

    public abstract T2 PartTwo();

    public override void Run()
    {
        if (Part == 1) WriteLn(PartOne());
        else WriteLn(PartTwo());
    }
}

public abstract class Puzzle<T> : Puzzle<T, T>
    where T : notnull
{
    public override T PartTwo() => PartOne();
}

public abstract class Puzzle : Puzzle<object>
{
    public override void Run()
    {
        // TODO Custom output type
        base.Run();
    }
}