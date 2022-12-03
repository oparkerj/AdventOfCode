using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Parsing;

namespace AdventOfCode2022.Puzzles;

public class Day3 : Puzzle<int>
{
    public int Score(char item)
    {
        return item.LetterIndex(LetterMode.LowerThenUpperOneIndexed);
    }
    
    public override int PartOne()
    {
        return Input.Sum(s =>
        {
            var (first, second) = s.SplitHalf();
            return first.Intersect(second).Sum(Score);
        });
    }

    public override int PartTwo()
    {
        return Input.Chunk(3)
            .Select(strings => strings.IntersectAll().TakeOne(Score))
            .Sum();
    }
}