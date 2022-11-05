using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day11 : Puzzle
{
    public Grid<int> Map = new();
    public Pos Bot = Pos.Origin;
    public Pos Dir = Pos.Up;

    public Day11()
    {
        Part = 2;
    }

    public void RunBot()
    {
        var c = Computer.From(InputLine);
        c.Input = () => Map[Bot];
        c.Output = new OutputSequence()
            .ThenInt(data => Map[Bot] = data)
            .ThenBool(dir => Dir = dir ? Dir.Clockwise() : Dir.CounterClockwise())
            .And(() => Bot += Dir)
            .Line;
        c.Execute();
    }
        
    public override void PartOne()
    {
        RunBot();
        WriteLn(Map.Count);
    }

    public override void PartTwo()
    {
        Map[Bot] = 1;
        RunBot();
        WriteLn(Map.ToArray().Stringify(i => i == 1 ? '#' : ' '));
    }
}