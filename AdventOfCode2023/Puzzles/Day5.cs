using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.New.Data;
using AdventToolkit.New.Extensions;

namespace AdventOfCode2023.Puzzles;

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
        var seedRanges = AllGroups[0][0].After(':')
            .Spaced()
            .Longs()
            .Chunk(2) // TODO custom chunk function
            .Select(pair => new Interval<long>(pair[0], pair[1]));
        
        var seeds = new MultiInterval<long> {seedRanges};

        foreach (var map in AllGroups.Skip(1))
        {
            var ranges = map.Skip(1).Select(s =>
            {
                Span<long> buf = stackalloc long[3];
                s.Spaced().Longs().ToSpan(buf);
                return (new Interval<long>(buf[0], buf[2]), new Interval<long>(buf[1], buf[2]));
            }).ToList();
            var current = new MultiInterval<long> {ranges.TupleSecond()};
            var next = new MultiInterval<long>();

            // Map ranges
            foreach (var (dest, source) in ranges)
            {
                foreach (var mappable in seeds.Intersect(source))
                {
                    next.Add(mappable with {Start = dest.Start + (mappable.Start - source.Start)});
                }
            }
            
            // Remove mapped seeds
            seeds.Remove(current);
            // Add unmapped seeds (map to self)
            next.Add(seeds);
            seeds = next;
        }
        
        return seeds.First().Start;
    }
}