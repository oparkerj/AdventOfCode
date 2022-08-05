using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day17 : Puzzle<int>
{
    public int[] Values;
    public const int Target = 150;

    public Day17() => Values = Input.Ints().ToArray();

    public override int PartOne()
    {
        return Algorithms.Sequences(Values.Length, 2, true)
            .Count(ints => ints.Zip(Values, Num.Mul).Sum() == Target);
    }

    public override int PartTwo()
    {
        return Algorithms.Sequences(Values.Length, 2, true)
            .MinsBy(ints => ints.Zip(Values, Num.Mul).Sum() == Target ? ints.Count(1) : int.MaxValue)
            .Count();
    }
}