using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day18 : Puzzle<int>
{
    public GameOfLife Game = new(true);

    public Day18()
    {
        var grid = Input.QuickMap('#', true, false).ToGrid();
        Game.CopySpace(grid);
        Game.WithLivingDeadRules(i => i is not (2 or 3), i => i == 3);
    }

    public override int PartOne()
    {
        Game.Step(100);
        return Game.CountActive();
    }

    public void ResetLights(GameOfLife<Pos, bool> game)
    {
        ((Grid<bool>) game.Space).Bounds.Corners().ForEach(pos => game[pos] = true);
    }

    public override int PartTwo()
    {
        ResetLights(Game);
        Game.StepAnd(100, ResetLights);
        return Game.CountActive();
    }
}