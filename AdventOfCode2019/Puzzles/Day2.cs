using System.Linq;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day2 : Puzzle
{
    public Day2()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var c = Computer.From(InputLine);
        c[1] = 12;
        c[2] = 2;
        c.Execute();
        WriteLn(c[0]);
    }

    public override void PartTwo()
    {
        var program = InputLine.Csv().Longs().ToArray();
        foreach (var sequence in Algorithms.Sequences(2, 100, true))
        {
            var noun = sequence[0];
            var verb = sequence[1];
            var c = new Computer(program.ToArray()) {[1] = noun, [2] = verb};
            c.Execute();
            if (c[0] == 19690720)
            {
                WriteLn(100 * noun + verb);
                return;
            }
        }
        WriteLn("None");
    }
}