using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day23 : Puzzle
    {
        public int Min = -1;
        public int Max = -1;
        
        public int Current;
        public readonly Dictionary<int, int> Next = new();

        public Day23()
        {
            ReadInput();
            Part = 2;
        }

        public void ReadInput()
        {
            var cups = Input[0].Select(c => c.ToString()).Ints().ToArray();
            var prev = cups[^1];
            foreach (var cup in cups)
            {
                if (cup < Min || Min == -1) Min = cup;
                if (cup > Max || Max == -1) Max = cup;
                Next[prev] = cup;
                prev = cup;
            }
            Current = cups[0];
        }

        public void Move()
        {
            var a = Next[Current];
            var b = Next[a];
            var c = Next[b];
            Next[Current] = Next[c];
            var dest = Current;
            do
            {
                dest--;
                if (dest < Min) dest = Max;
            } while (a == dest || b == dest || c == dest);
            Next[c] = Next[dest];
            Next[dest] = a;
            Current = Next[Current];
        }
        
        public override void PartOne()
        {
            100.Times(Move);
            var result = "";
            var c = 1;
            foreach (var _ in ..(Max - 1))
            {
                result += c = Next[c];
            }
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var last = Next.Single(pair => pair.Value == Current).Key;
            for (var i = Max + 1; i <= 1_000_000; i++)
            {
                Next[last] = i;
                last = i;
            }
            Next[1_000_000] = Current;
            Max = 1_000_000;
            10_000_000.Times(Move);
            var a = Next[1];
            var result = (long) a * Next[a];
            WriteLn(result);
        }
    }
}