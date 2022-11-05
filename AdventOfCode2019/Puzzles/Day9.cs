using AdventOfCode2019.IntCode;
using AdventToolkit;

namespace AdventOfCode2019.Puzzles;

public class Day9 : Puzzle
{
    public override void PartOne()
    {
        var c = Computer.From(InputLine);
        c.Input = Computer.ConsoleReader();
        c.Output = Computer.ConsoleOutput();
        c.Execute();
    }
}