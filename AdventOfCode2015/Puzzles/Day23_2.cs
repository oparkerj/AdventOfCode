using System.Numerics;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Computer;

namespace AdventOfCode2015.Puzzles;

public class Day23_2 : Puzzle<BigInteger>
{
    public Day23_2()
    {
        // Part = 1;
        InputName = "Day23.txt";
    }

    public override BigInteger PartOne()
    {
        var builder = PrefixInstructionBuilder<BigInteger>.Default(BigInteger.Parse);
        builder.Add("hlf r", r => r.Value /= 2);
        builder.Add("tpl r", r => r.Value *= 3);
        builder.Add("inc r", r => r.Value++);
        builder.AddCpu("jmp d", (cpu, d) => cpu.JumpRelative(d));
        builder.AddCpu("jie r, d", (cpu, r, d) =>
        {
            if (r.Value % 2 == 0) cpu.JumpRelative(d);
        });
        builder.AddCpu("jio r, d", (cpu, r, d) =>
        {
            if (r.Value == 1) cpu.JumpRelative(d);
        });

        var cpu = Cpu<BigInteger>.StandardRegisters(2);
        cpu.InstructionSet = builder.BuildAndParseAll(cpu, Input);

        if (Part == 2) cpu.Memory['a'] = 1;
        cpu.Execute();

        return cpu.Memory['b'];
    }
}