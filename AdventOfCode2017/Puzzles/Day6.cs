using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day6 : Puzzle
    {
        public Day6()
        {
            Part = 2;
        }

        public void Redistribute(int[] blocks)
        {
            var max = blocks.FindMax();
            var count = blocks[max];
            blocks[max] = 0;
            for (var i = 0; i < count; i++)
            {
                blocks[(max + i + 1).CircularMod(blocks.Length)]++;
            }
        }

        public override void PartOne()
        {
            var blocks = InputLine.Tabbed().Ints().ToArray();
            var (offset, cycle) = Algorithms.FindCyclePeriod(blocks, arr => arr.ToCsv(), Redistribute);
            WriteLn(offset + cycle);
        }

        public override void PartTwo()
        {
            var blocks = InputLine.Tabbed().Ints().ToArray();
            var (_, cycle) = Algorithms.FindCyclePeriod(blocks, arr => arr.ToCsv(), Redistribute);
            WriteLn(cycle);
        }
    }
}