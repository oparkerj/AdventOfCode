namespace AdventToolkit.Common;

public interface IToPos<out T>
{
    T ToPos();
}

public readonly record struct Int2(int A, int B) : IToPos<Pos>
{
    public Pos ToPos() => new(A, B);
}

public readonly record struct Int3(int A, int B, int C) : IToPos<Pos3D>
{
    public Pos3D ToPos() => new(A, B, C);
}

public readonly record struct Int4(int A, int B, int C, int D) : IToPos<Pos4D>
{
    public Pos4D ToPos() => new(A, B, C, D);
}