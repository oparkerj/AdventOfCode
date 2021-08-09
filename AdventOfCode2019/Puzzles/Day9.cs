using AdventOfCode2019.IntCode;
using AdventToolkit;

namespace AdventOfCode2019.Puzzles
{
    public class Day9 : Puzzle
    {
        public override void PartOne()
        {
            var c = Computer.From(InputLine);
            c.LineIn = Computer.ConsoleReader();
            c.LineOut = Computer.ConsoleOutput();
            c.Execute();
        }
    }
}