using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Computer;

namespace AdventOfCode2022.Puzzles;

public class Day10 : Puzzle<int, string>
{
    public void Execute(Action<int> addCycle)
    {
        var builder = PrefixInstructionBuilder<int>.Default();
        builder.AddCpu("addx d", (cpu, v) =>
        {
            addCycle(cpu.Memory[0]);
            addCycle(cpu.Memory[0]);
            cpu.Memory[0] += v;
        });
        builder.AddCpu("noop", cpu => addCycle(cpu.Memory[0]));

        var cpu = Cpu<int>.StandardRegisters(1);
        cpu.InstructionSet = builder.BuildAndParseAll(cpu, Input);
        cpu.Memory[0] = 1;

        cpu.Execute();
    }

    public override int PartOne()
    {
        var result = 0;
        var cycles = 0;
        
        Execute(current =>
        {
            cycles++;
            if (cycles is 20 or 60 or 100 or 140 or 180 or 220)
            {
                result += cycles * current;
            }
        });
        
        return result;
    }

    public override string PartTwo()
    {
        var result = new Grid<char>();
        result.Default = ' ';

        var cycles = 0;

        Execute(x =>
        {
            cycles++;
            var pos = new Pos((cycles - 1) % 40, cycles / 40);
            if (Interval.RangeInclusive(x - 1, x + 1).Contains(pos.X))
            {
                result[pos] = '#';
            }
        });

        return result.ToArray(false).Stringify();
    }
}