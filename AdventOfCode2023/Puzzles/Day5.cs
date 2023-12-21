using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day5 : Puzzle<long>
{
    public override long PartOne()
    {
        var seeds = AllGroups[0][0].After(':').Spaced().Longs().ToList();

        return seeds.Min(GetMapping);

        long GetMapping(long seed)
        {
            foreach (var map in AllGroups.Skip(1))
            {
                var ranges = map.Skip(1).Select(s =>
                {
                    var mapping = s.Spaced().Longs().ToList();
                    return (mapping[0], mapping[1], mapping[2]);
                });
                var seedValue = seed;
                var (dest, source, _) = ranges.FirstOrDefault(tuple => seedValue >= tuple.Item2 && seedValue < tuple.Item2 + tuple.Item3, (seed, seed, 1));
                seed = dest + (seed - source);
            }
            return seed;
        }
    }

    public override long PartTwo()
    {
        // Part two solved in toolkit branch
        return base.PartTwo();
    }
}