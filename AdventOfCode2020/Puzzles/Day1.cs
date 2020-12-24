using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day1 : Puzzle
    {
        public Day1()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var nums = Input.Ints().ToArray();
            var product = nums.NSum(2, 2020).GetFrom(nums).Product();
            WriteLn(product);
        }

        public override void PartTwo()
        {
            var nums = Input.Ints().ToArray();
            var product = nums.NSum(3, 2020).GetFrom(nums).Product();
            WriteLn(product);
        }
    }
}