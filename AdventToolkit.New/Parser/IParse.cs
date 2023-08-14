namespace AdventToolkit.New.Parser;

public interface IParse<out T>
{
    static abstract T Parse(ReadOnlySpan<char> span);
}