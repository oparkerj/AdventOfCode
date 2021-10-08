using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;

namespace AdventOfCode2018.Puzzles
{
    public class Day18 : Puzzle
    {
        public const char Open = '.';
        public const char Trees = '|';
        public const char Lumber = '#';

        public Day18()
        {
            Part = 2;
            Measure = true;
        }

        public GameOfLife<Pos, char> CreateGame()
        {
            return GameOfLife.OnGrid(Open, Trees, true)
                .WithUpdate(cell =>
                {
                    return cell.State switch
                    {
                        Open when cell.CountNear(Trees) >= 3 => Trees,
                        Trees when cell.CountNear(Lumber) >= 3 => Lumber,
                        Lumber when cell.CountNear(Lumber) >= 1 && cell.CountNear(Trees) >= 1 => Lumber,
                        Lumber => Open,
                        _ => cell.State,
                    };
                });
        }

        public override void PartOne()
        {
            var game = CreateGame();
            game.ReadFrom(Input.ToGrid());
            game.Step(10);
            var result = game.CountState(Trees) * game.CountState(Lumber);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var game = CreateGame();
            var input = Input.ToGrid();
            var area = new char[50, 50];
            game.ReadFrom(input);
            var (offset, cycle) = Algorithms.FindCyclePeriod(game, life => life.CopyTo(input).ToArray(area).Stringify(), life => life.Step());
            
            // Reset and simulate to the point where it is equal to the ending
            game.ReadFrom(Input.ToGrid());
            var ending = 1000000000.CycleOffset((int) offset, (int) cycle);
            game.Step(ending);
            
            var result = game.CountState(Trees) * game.CountState(Lumber);
            WriteLn(result);
        }
    }
}