using System;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day5 : Puzzle
    {
        public Day5()
        {
            Part = 2;
        }

        public int ToInt(string id)
        {
            id = id.Replace('F', '0');
            id = id.Replace('B', '1');
            id = id.Replace('L', '0');
            id = id.Replace('R', '1');
            return Convert.ToInt32(id, 2);
        }

        public int GetId(int i)
        {
            return (i >> 3) * 8 + (i & 7);
        }

        public override void PartOne()
        {
            WriteLn(Input.Select(ToInt).Select(GetId).Max());
        }

        public override void PartTwo()
        {
            // Find the missing id in the given min/max range
            var total = Input.Select(ToInt).Select(GetId).Sum(out var min, out var max);
            // Result is sum of values that should exist minus
            // sum of values that actually exist
            WriteLn(Algorithms.SumRange(min, max) - total);
        }
    }
}