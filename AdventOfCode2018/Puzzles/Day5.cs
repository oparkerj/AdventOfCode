using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles
{
    public class Day5 : Puzzle
    {
        public const int Offset = 'a' - 'A';

        public Day5()
        {
            Part = 2;
        }

        public int Reduce(IEnumerable<char> source)
        {
            var chain = source.ToLinkedList();
            var current = chain.First;
            while (current != null)
            {
                var next = current.Next;
                if (next == null) break;
                if (current.Value.Max(next.Value) - current.Value.Min(next.Value) == Offset)
                {
                    var nextCurrent = current.Previous;
                    chain.Remove(current);
                    chain.Remove(next);
                    current = nextCurrent ?? chain.First;
                }
                else current = current.Next;
            }
            return chain.Count;
        }

        public override void PartOne()
        {
            WriteLn(Reduce(InputLine));
        }

        public override void PartTwo()
        {
            var result = Enumerable.Range('a', 26).Min(i => Reduce(InputLine.Where(c => char.ToLower(c) != i)));
            WriteLn(result);
        }
    }
}