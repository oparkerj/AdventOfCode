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
            var nums = Input.Select(int.Parse).ToArray();
            var product = nums.Get(nums.NSum(2, 2020)).Product();
            WriteLn(product);
            // for (var i = 0; i < nums.Length; i++)
            // {
            //     for (var j = i + 1; j < nums.Length; j++)
            //     {
            //         if (nums[i] + nums[j] != 2020) continue;
            //         WriteLn(nums[i] * nums[j]);
            //         break;
            //     }
            // }
        }

        public override void PartTwo()
        {
            var nums = Input.Select(int.Parse).ToArray();
            var product = nums.Get(nums.NSum(3, 2020)).Product();
            WriteLn(product);
            // for (var i = 0; i < nums.Length; i++)
            // {
            //     for (var j = i + 1; j < nums.Length; j++)
            //     {
            //         for (var k = j + 1; k < nums.Length; k++)
            //         {
            //             if (nums[i] + nums[j] + nums[k] != 2020) continue;
            //             WriteLn(nums[i] * nums[j] * nums[k]);
            //             break;
            //         }
            //     }
            // }
        }
    }
}