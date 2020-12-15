using System.Linq;
using AdventToolkit;

namespace AdventOfCode2020.Puzzles
{
    public class Day3 : Puzzle
    {
        public Day3()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var width = Input[0].Length;
            var count = Input.Select((s, i) => s[(i * 3) % width]).Count(c => c == '#');
            WriteLn(count);
        }

        public int Count(string[] trees, int right, int down)
        {
            var width = trees[0].Length;
            return trees.Where((s, i) => i % down == 0 && s[(i / down * right) % width] == '#').Count();
        }

        public override void PartTwo()
        {
            var count = (long) Count(Input, 1, 1);
            count *= Count(Input, 3, 1);
            count *= Count(Input, 5, 1);
            count *= Count(Input, 7, 1);
            count *= Count(Input, 1, 2);
            WriteLn(count);
        }
    }
}