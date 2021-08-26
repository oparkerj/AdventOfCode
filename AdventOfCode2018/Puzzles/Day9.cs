using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2018.Puzzles
{
    public class Day9 : Puzzle
    {
        public Day9()
        {
            Part = 2;
        }

        public (int Players, int Last) GetInput()
        {
            return InputLine.Extract<(int, int)>(@"(\d+) players; last marble is worth (\d+) points");
        }

        public long Simulate(int size, int last)
        {
            var players = new long[size];
            var circle = new LinkedList<int>();
            circle.AddFirst(0);
            var current = circle.First;

            foreach (var i in Enumerable.Range(1, last))
            {
                if (i % 23 == 0)
                {
                    var player = (i - 1) % size;
                    players[player] += i;
                    var remove = current.Repeat(node => node.PreviousCircular(), 7);
                    current = remove.NextCircular();
                    players[player] += remove!.Value;
                    circle.Remove(remove);
                }
                else
                {
                    current = circle.AddAfter(current.NextCircular(), i);
                }
            }
            return players.Max();
        }

        public override void PartOne()
        {
            var (size, last) = GetInput();
            WriteLn(Simulate(size, last));
        }

        public override void PartTwo()
        {
            var (size, last) = GetInput();
            WriteLn(Simulate(size, last * 100));
        }
    }
}