using System.Linq;
using AdventToolkit;
using AdventToolkit.Data;

namespace AdventOfCode2020.Puzzles
{
    public class Day18 : Puzzle
    {
        public Day18()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var reader = Expression<long>.Reader()
                .ForConstants()
                .AddBinaryOp("+", (a, b) => a + b, 1)
                .AddBinaryOp("*", (a, b) => a * b, 1);
            var sum = Input.Select(reader.GetResult).Sum();
            WriteLn(sum);
        }

        public override void PartTwo()
        {
            var reader = Expression<long>.Reader()
                .ForConstants()
                .AddBinaryOp("+", (a, b) => a + b, 2)
                .AddBinaryOp("*", (a, b) => a * b, 1);
            var sum = Input.Select(reader.GetResult).Sum();
            WriteLn(sum);
        }
    }
}