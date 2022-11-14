using System.Linq;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;

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

public class Day9_2 : Puzzle<long>
{
    public override long PartOne()
    {
        var c = new IntCodeCpu(InputLine);
        var data = new DataLink();
        data.Link(c);
        data.Insert(1);
        c.Execute();

        var output = Collect<long>.Out(data.TryTake).ToArray();
        foreach (var wrong in output[..^1])
        {
            WriteLn($"Bad opcode: {wrong}");
        }

        return output[^1];
    }

    public override long PartTwo()
    {
        var c = new IntCodeCpu(InputLine);
        var data = new DataLink();
        data.Link(c);
        data.Insert(2);
        c.Execute();
        return data.Take();
    }
}