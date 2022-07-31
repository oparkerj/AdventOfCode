using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day17 : Puzzle
{
    public int[] Values;
    public const int Target = 150;

    public Day17() => Values = Input.Ints().ToArray();

    public override void PartOne()
    {
        var result = Algorithms.Sequences(Values.Length, 2, true)
            .Count(ints => ints.Zip(Values, Num.Mul).Sum() == Target);
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var result = Algorithms.Sequences(Values.Length, 2, true)
            .MinsBy(ints => ints.Zip(Values, Num.Mul).Sum() == Target ? ints.Count(1) : int.MaxValue)
            .Count();
        WriteLn(result);
    }
}