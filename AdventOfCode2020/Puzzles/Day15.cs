using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day15 : Puzzle
    {
        public int[] Start;

        public Day15()
        {
            Start = Input[0].Csv().Ints().ToArray();
            Part = 1;
        }

        public IEnumerable<int> Sequence()
        {
            var nums = new Dictionary<int, (int diff, int turn)>();
            var turn = 0;
            var last = 0;
            while (true)
            {
                if (turn < Start.Length)
                {
                    nums[Start[turn]] = (0, turn);
                    yield return last = Start[turn];
                }
                else
                {
                    var (answer, _) = nums[last];
                    nums[answer] = nums.TryGetValue(answer, out var info) ? (turn - info.turn, turn) : (0, turn);
                    yield return last = answer;
                }
                turn++;
            }
        }

        public override void PartOne()
        {
            var find = 2020;
            WriteLn(Sequence().ElementAt(find - 1));
        }

        public override void PartTwo()
        {
            var find = 30_000_000;
            WriteLn(Sequence().ElementAt(find - 1));
        }
    }
}