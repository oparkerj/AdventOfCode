using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day18 : Puzzle
{
    public GameOfLife Game = new(true);

    public Day18()
    {
        var grid = Input.QuickMap('#', true, false).ToGrid();
        Game.CopySpace(grid);
        Game.WithLivingDeadRules(i => i is not (2 or 3), i => i == 3);
    }

    public override void PartOne()
    {
        Game.Step(100);
        WriteLn(Game.CountActive());
    }

    public void ResetLights(GameOfLife<Pos, bool> game)
    {
        ((Grid<bool>) game.Space).Bounds.Corners().ForEach(pos => game[pos] = true);
    }

    public override void PartTwo()
    {
        ResetLights(Game);
        Game.StepAnd(100, ResetLights);
        WriteLn(Game.CountActive());
    }
}