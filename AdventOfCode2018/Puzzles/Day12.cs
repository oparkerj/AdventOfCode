using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq;

namespace AdventOfCode2018.Puzzles
{
    public class Day12 : Puzzle
    {
        public readonly string[][] Sections;
        public readonly Dictionary<int, bool> Rules = new();

        public Day12()
        {
            Sections = Groups.ToArray();
            foreach (var s in Sections[1])
            {
                Rules[Plants(s[..5]).ToInt()] = s[^1] == '#';
            }
            Part = 2;
        }

        public IEnumerable<bool> Plants(string s)
        {
            return s.Select(c => c == '#');
        }

        public GameOfLife<int> MakeGame()
        {
            var game = new GameOfLife<int>(() => new Line<bool>());
            game.WithExpansion();
            game.WithUpdateFunction((p, _, _) =>
            {
                var pattern = new Interval(p - 2, 5).Select(i => game[i]).ToInt();
                return Rules[pattern];
            });
            foreach (var (i, p) in Plants(Sections[0][0][15..]).Index())
            {
                game[i] = p;
            }
            return game;
        }

        public long Total(GameOfLife<int> game)
        {
            return game.WhereValue(true).Keys().Sum();
        }

        public override void PartOne()
        {
            var game = MakeGame();
            game.Step(20);
            var result = game.WhereValue(true).Keys().Sum();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var game = MakeGame();

            var count = 50_000_000_000;
            var last = new CircularBuffer<long>(Rules.Count);
            var old = Total(game);
            for (var i = 0L; i < count; i++)
            {
                game.Step();
                var sum = Total(game);
                last.Add(sum - old);
                // Detect when delta converges to a single value
                if (last.AllEqual(sum - old))
                {
                    old = (count - i - 1) * (sum - old) + sum;
                    break;
                }
                old = sum;
            }

            WriteLn(old);
        }
    }
}