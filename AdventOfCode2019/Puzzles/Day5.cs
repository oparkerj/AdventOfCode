using AdventOfCode2019.IntCode;
using AdventToolkit;

namespace AdventOfCode2019.Puzzles;

public class Day5 : Puzzle
{
    public Day5()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var c = Computer.From(InputLine);
        var data = new DataLink();
        c.LineIn = data.Output;
        c.LineOut = Computer.ConsoleOutput();
        data.Insert(1);
        c.Execute();
    }

    public override void PartTwo()
    {
        var c = Computer.From(InputLine);
        var data = new DataLink();
        c.LineIn = data.Output;
        c.LineOut = Computer.ConsoleOutput();
        data.Insert(5);
        c.Execute();
    }
}