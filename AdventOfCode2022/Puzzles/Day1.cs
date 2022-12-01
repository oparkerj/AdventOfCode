using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day1 : Puzzle<int>
{
    public override int PartOne()
    {
        return AllGroups.Select(strings => strings.Ints().Sum()).Max();
    }

    public override int PartTwo()
    {
        return AllGroups.Select(strings => strings.Ints().Sum()).Order().TakeLast(3).Sum();
    }
}