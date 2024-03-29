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
        c.Input = data.Output;
        c.Output = Computer.ConsoleOutput();
        data.Insert(1);
        c.Execute();
    }

    public override void PartTwo()
    {
        var c = Computer.From(InputLine);
        var data = new DataLink();
        c.Input = data.Output;
        c.Output = Computer.ConsoleOutput();
        data.Insert(5);
        c.Execute();
    }
}

public class Day5_2 : Improve<Day5, long>
{
    public int Id = 1;

    public override long PartOne()
    {
        var c = new IntCodeCpu(InputLine);
        var link = new DataLink();
        c.Input = link.Output;
        c.Output = link.Input;
        link.Insert(Id);
        c.Execute();
        return link.TakeLast();
    }

    public override long PartTwo()
    {
        Id = 5;
        return PartOne();
    }
}