using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day4 : Puzzle<int>
{
    public override int PartOne()
    {
        return Input.Extract<Int4>(Patterns.UInt4)
            .Count(ints =>
            {
                var first = Interval.RangeInclusive(ints.A, ints.B);
                var second = Interval.RangeInclusive(ints.C, ints.D);
                return first.ContainsOrInside(second);
            });
    }

    public override int PartTwo()
    {
        return Input.Extract<Int4>(Patterns.UInt4)
            .Count(ints =>
            {
                var first = Interval.RangeInclusive(ints.A, ints.B);
                var second = Interval.RangeInclusive(ints.C, ints.D);
                return first.Overlaps(second);
            });
    }
}