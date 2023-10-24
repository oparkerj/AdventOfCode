using AdventToolkit.New;

namespace Benchmarks;

public class TestPuzzle : Puzzle<int>
{
    public override string GetInput() => string.Empty;

    public override int PartOne()
    {
        var array = Enumerable.Range(0, 5000).ToArray();
        Array.Reverse(array);
        return array.Sum();
    }
}