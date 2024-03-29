using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day24 : Puzzle<long>
{
    public List<int> Weights;

    public Day24()
    {
        Weights = Input.Ints().ToList();
    }

    public override long PartOne()
    {
        var groups = Part == 2 ? 4 : 3;
        var target = Weights.Sum() / groups;

        for (var i = 1;; i++)
        {
            var result = Weights.Subsets(i)
                .Where(ints => ints.Sum() == target)
                .Select(ints => ints.LongProduct())
                .DefaultIfEmpty()
                .Min();
            if (result == 0) continue;
            return result;
        }
    }
}