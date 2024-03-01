namespace Testing;

public static class TestExtensions
{
    public static IEnumerable<object[]> Expand<T>(params T[] args)
    {
        foreach (var item in args)
        {
            yield return [item];
        }
    }
}