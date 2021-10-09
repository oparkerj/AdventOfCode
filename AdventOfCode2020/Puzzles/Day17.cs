using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;

namespace AdventOfCode2020.Puzzles
{
    public class Day17 : Puzzle
    {
        public Day17()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var game = new GameOfLife<Pos3D>()
                .WithNeighborFunction(pos => pos.Around())
                .WithLivingDeadRules(i => i is < 2 or > 3, i => i == 3)
                .WithExpansion()
                .WithKeepDead(false);
            foreach (var ((x, y), c) in Input.As2D())
            {
                game[(x, y, 0)] = c == '#';
            }
            game.Step(6);
            WriteLn(game.CountActive());
        }

        public override void PartTwo()
        {
            var game = new GameOfLife<(int, int, int, int)>()
                .WithNeighborFunction(pos => pos.Around())
                .WithLivingDeadRules(i => i is < 2 or > 3, i => i == 3)
                .WithExpansion()
                .WithKeepDead(false);
            foreach (var ((x, y), c) in Input.As2D())
            {
                game[(x, y, 0, 0)] = (c == '#');
            }
            game.Step(6);
            WriteLn(game.CountActive());
        }
    }
}