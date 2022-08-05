using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day14 : Puzzle<int>
{
    public const int TimeLimit = 2503;

    public record Reindeer(int Speed, int Time, int Rest)
    {
        public int ComputeDistance(int time)
        {
            var (chunks, offset) = Math.DivRem(time, Time + Rest);
            return Speed * Time * chunks + Speed * Math.Min(offset, Time);
        }
    }
    
    public override int PartOne()
    {
        return Input.Extract<Reindeer>(Patterns.Int3).Select(r => r.ComputeDistance(TimeLimit)).Max();
    }

    public override int PartTwo()
    {
        var reindeer = Input.Extract<Reindeer>(Patterns.Int3).ToList();
        var points = new int[reindeer.Count];
        foreach (var i in Enumerable.Range(1, TimeLimit))
        {
            var winners = reindeer.Index()
                .MaxsBy(pair => pair.Value.ComputeDistance(i))
                .Keys();
            foreach (var winner in winners)
            {
                points[winner]++;
            }
        }
        return points.Max();
    }
}