using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return AllGroups.ParseInner<int>().SumInner().Max();
    }

    public override int PartTwo()
    {
        return AllGroups.ParseInner<int>().SumInner().TakeMax(3).Sum();
    }
}