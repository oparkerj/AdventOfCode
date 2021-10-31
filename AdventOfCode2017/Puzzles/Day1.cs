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
            var result = InputLine.Digits().Again(1).Pairwise((a, b) => a == b ? a : 0).Sum();
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