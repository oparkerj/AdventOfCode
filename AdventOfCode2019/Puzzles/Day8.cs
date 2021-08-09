using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day8 : Puzzle
    {
        public const int Width = 25;
        public const int Height = 6;

        public Day8()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var layer = InputLine.Ints()
                .Batch(Height * Width)
                .Select(ints => ints.Frequencies().ToDictionary())
                .OrderBy(layer => layer[0])
                .First();
            WriteLn(layer[1] * layer[2]);
        }

        public override void PartTwo()
        {
            InputLine.Ints()
                .Batch(Height * Width)
                .Aggregate((a, b) => a.Zip(b)
                    .Select(tuple => tuple.First is 0 or 1 ? tuple.First : tuple.Second))
                .Batch(Width)
                .Select(ints => string.Concat(ints.QuickMap(1, '#', ' ')))
                .ForEach(WriteLn);
        }
    }
}