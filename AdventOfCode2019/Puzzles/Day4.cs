using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day4 : Puzzle
    {
        public readonly int Upper;
        public readonly int Lower;
        
        public Day4()
        {
            var input = InputLine.Split('-').Ints().ToArray();
            Lower = input[0];
            Upper = input[1];
            Part = 2;
        }

        private bool Valid(string s)
        {
            return s.Sorted().Str() == s &&
                   s.RunLengthEncode().Any(pair => pair.Value > 1);
        }

        public override void PartOne()
        {
            var count = Enumerable.Range(Lower, Upper - Lower + 1).ToStrings().Count(Valid);
            WriteLn(count);
        }

        private bool Valid2(string s)
        {
            return s.Sorted().Str() == s &&
                   s.RunLengthEncode().Any(pair => pair.Value == 2);
        }

        public override void PartTwo()
        {
            var count = Enumerable.Range(Lower, Upper - Lower + 1).ToStrings().Count(Valid2);
            WriteLn(count);
        }
    }
}