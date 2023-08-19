namespace AdventToolkit.New.Transform;

public interface IParse<out T>
{
    static abstract T Parse(ReadOnlySpan<char> span);
}