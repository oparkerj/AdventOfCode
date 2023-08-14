namespace AdventToolkit.New;

public abstract class Puzzle : PuzzleBase
{
    public abstract void PartOne();

    public virtual void PartTwo() => PartOne();

    public override void Run()
    {
        if (Part == 1) PartOne();
        else PartTwo();
    }
}

public abstract class Puzzle<T1, T2> : PuzzleBase
    where T1 : notnull
    where T2 : notnull
{
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