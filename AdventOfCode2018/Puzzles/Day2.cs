using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2018.Puzzles
{
    public class Day2 : Puzzle
    {
        public Day2()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var (twos, threes) = Input.Select(id => id.Frequencies().ToDictionary())
                .Select(counts => new Pos(counts.ContainsValue(2).AsInt(), counts.ContainsValue(3).AsInt()))
                .Sum();
            WriteLn(twos * threes);
        }

        public override void PartTwo()
        {
            foreach (var pair in Algorithms.SequencesIncreasing(2, Input.Length, true))
            {
                var ids = Input.Get(pair).ToArray();
                var diff = ids[0].FirstDifference(ids[1]);
                var a = ids[0].Exclude(diff, 1).Str();
                if (a == ids[1].Exclude(diff, 1).Str())
                {
                    WriteLn(a);
                    return;
                }
            }
            WriteLn("None");
        }
    }
}