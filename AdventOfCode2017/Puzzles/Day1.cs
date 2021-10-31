using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles
{
    public class Day1 : Puzzle
    {
        public Day1()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var result = InputLine.Digits().Pairwise((a, b) => a == b ? a : 0).Sum();
            if (InputLine[0] == InputLine[^1]) result += InputLine[0].AsInt();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var half = InputLine.Length / 2;
            var captcha = new CircularBuffer<int>(InputLine.Digits().ToArray());
            var result = captcha.Select((d, i) => d == captcha[i + half] ? d : 0).Sum();
            WriteLn(result);
        }
    }
}