using System.Collections;
using AdventToolkit.Solvers;

namespace AdventOfCode2021.Puzzles;

public class Day20 : Puzzle
{
    public BitArray Key;
    public GameOfLife Game;

    public Day20()
    {
        Part = 2;
        
        Key = InputLine.ToBitArray('#');
        Game = new GameOfLife(true);
        Game.Expanding = true;
        Game.NeighborFunction = pos => pos.RectAround(1, 1);
        Game.UpdateFunction = cell =>
        {
            var index = cell.Neighbors().BitsToInt();
            return Key[index];
        };
        var input = AllGroups[1].Select2D(c => c == '#').ToGrid();
        foreach (var (pos, value) in input)
        {
            Game[pos] = value;
        }
    }

    public void Step(int times)
    {
        for (var i = 0; i < times; i++)
        {
            Game.Step();
            // If the algorithm says that all-off or all-on sections change state, we
            // update the default value for pixels outside the image. This effectively
            // performs the update on the infinite pixels outside the image.
            if (!Game.Default && Key[0]) Game.Default = true;
            else if (Game.Default && !Key[^1]) Game.Default = false;
        }
    }

    public override void PartOne()
    {
        Step(2);
        WriteLn(Game.CountValues(true));
    }

    public override void PartTwo()
    {
        Step(50);
        WriteLn(Game.CountValues(true));
    }
}