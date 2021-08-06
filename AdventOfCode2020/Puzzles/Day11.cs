using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2020.Puzzles
{
    public class Day11 : Puzzle
    {
        public const int Floor = 0;
        public const int Empty = 1;
        public const int Taken = 2;
        
        public Day11()
        {
            Part = 2;
        }

        public int Map(char c)
        {
            return c == 'L' ? Empty : Floor;
        }

        public override void PartOne()
        {
            var game = new GameOfLife<Pos, int>(Empty, Taken)
                .WithNeighborFunction(Grid<int>.Around())
                .WithLivingDeadRules(i => i >= 4, i => i == 0);
            foreach (var (pos, c) in Input.As2D())
            {
                game[pos] = Map(c);
            }
            game.StepUntil(i => i == 0);
            WriteLn(game.CountValues(Taken));
        }

        public override void PartTwo()
        {
            Pos[] dirs = (0, 0).Around().ToArray();
            var game = new GameOfLife<Pos, int>(Empty, Taken);
            game.WithNeighborFunction(pos => dirs.ToTrace(pos, game, (_, i) => i > Floor))
                .WithLivingDeadRules(i => i >= 5, i => i == 0);
            foreach (var (pos, c) in Input.As2D())
            {
                game[pos] = Map(c);
            }
            game.StepUntil(i => i == 0);
            WriteLn(game.CountValues(Taken));
        }
    }
}