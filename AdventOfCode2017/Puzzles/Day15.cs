using System.Linq;
using System.Numerics;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2017.Puzzles
{
    public class Day15 : Puzzle
    {
        public Day15()
        {
            Part = 2;
        }

        public (BigInteger, BigInteger) GetFactors()
        {
            var array = Input.Extract<int>(@".+?(\d+)").ToArray(2);
            return (array[0], array[1]);
        }

        public override void PartOne()
        {
            var (a, b) = GetFactors();
            var (af, bf) = (16807, 48271);
            var counter = 0;
            foreach (var _ in Enumerable.Range(0, 40_000_000))
            {
                a = a * af % 2147483647;
                b = b * bf % 2147483647;
                if ((a & 0xFFFF) == (b & 0xFFFF)) counter++;
            }
            WriteLn(counter);
        }

        public override void PartTwo()
        {
            var (a, b) = GetFactors();
            var (af, bf) = (16807, 48271);
            var counter = 0;
            foreach (var _ in Enumerable.Range(0, 5_000_000))
            {
                do
                {
                    a = a * af % 2147483647;
                } while (a % 4 != 0);
                do
                {
                    b = b * bf % 2147483647;
                } while (b % 8 != 0);
                if ((a & 0xFFFF) == (b & 0xFFFF)) counter++;
            }
            WriteLn(counter);
        }
    }
}