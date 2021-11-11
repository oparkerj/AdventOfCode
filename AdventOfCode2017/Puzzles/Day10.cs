using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles
{
    public class Day10 : Puzzle
    {
        public Day10()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            const int size = 256;
            var nums = Enumerable.Range(0, size).Repeat(2).ToArray();
            var current = 0;
            var skip = 0;
            foreach (var length in InputLine.Csv().Ints())
            {
                new Span<int>(nums, current, length).Reverse();
                if (current + length > size)
                {
                    Array.Copy(nums, size, nums, 0, current + length - size);
                    Array.Copy(nums, current, nums, current + size, size - current);
                }
                else
                {
                    Array.Copy(nums, current, nums, current + size, length);
                }
                current = (current + length + skip) % size;
                skip++;
            }
            var result = nums[0] * nums[1];
            WriteLn(result);
        }

        public static string KnotHash(string s)
        {
            const int size = 256;
            var input = s.Ascii().ConcatMany(17, 31, 73, 47, 23).ToArray();
            var nums = Enumerable.Range(0, size).Repeat(2).ToArray();
            var current = 0;
            var skip = 0;
            for (var i = 0; i < 64; i++)
            {
                foreach (var length in input)
                {
                    new Span<int>(nums, current, length).Reverse();
                    if (current + length > size)
                    {
                        Array.Copy(nums, size, nums, 0, current + length - size);
                        Array.Copy(nums, current, nums, current + size, size - current);
                    }
                    else
                    {
                        Array.Copy(nums, current, nums, current + size, length);
                    }
                    current = (current + length + skip) % size;
                    skip++;
                }
            }
            return nums.Take(size)
                .Batch(16)
                .Select(ints => ints.Aggregate(Num.Xor).ToString("x2"))
                .Str();
        }

        public override void PartTwo()
        {
            WriteLn(KnotHash(InputLine));
        }
    }
}