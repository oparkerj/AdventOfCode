using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day2 : Puzzle
    {
        public Day2()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var result = Input.Select(s => s.Split('\t').Ints().Extents().Select((min, max) => max - min)).Sum();
            WriteLn(result);
        }

        public int EvenDivision((int A, int B) pair)
        {
            if (pair.A.Divides(pair.B)) return pair.A / pair.B;
            if (pair.B.Divides(pair.A)) return pair.B / pair.A;
            return 0;
        }

        public override void PartTwo()
        {
            var result = Input.SelectMany(s => s.Split('\t').Ints().ToArray().Pairs().Select(EvenDivision)).Sum();
            WriteLn(result);
        }
    }
}