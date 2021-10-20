using System.Linq;
using AdventToolkit;
using AdventToolkit.Utilities.Parsing;

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
            var expr = new SingleTypeExpression<long>(long.Parse);
            expr.AddBinary(new BinarySymbol("+", 1), (a, b) => a + b);
            expr.AddBinary(new BinarySymbol("*", 1), (a, b) => a * b);
            var sum = Input.Select(expr.Eval).Sum();
            WriteLn(sum);
        }

        public override void PartTwo()
        {
            var expr = new SingleTypeExpression<long>(long.Parse);
            expr.AddBinary(new BinarySymbol("+", 2), (a, b) => a + b);
            expr.AddBinary(new BinarySymbol("*", 1), (a, b) => a * b);
            var sum = Input.Select(expr.Eval).Sum();
            WriteLn(sum);
        }
    }
}