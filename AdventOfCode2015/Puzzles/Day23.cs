using System.Numerics;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using AdventToolkit.Utilities.Computer.Builders.Opcode;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventOfCode2015.Puzzles;

public class Day23 : Puzzle<BigInteger>
{
    public BigInteger[] Register = new BigInteger[2];
    public int Ptr = 0;
    
    public override BigInteger PartOne()
    {
        var matcher = new StringMatcher();
        matcher.AddPrefix<char>("hlf", "hlf (.)", Half);
        matcher.AddPrefix<char>("tpl", "tpl (.)", Triple);
        matcher.AddPrefix<char>("inc", "inc (.)", Increment);
        matcher.AddPrefix<int>("jmp", Patterns.Int.ToString(), Jump);
        matcher.AddPrefix<char, int>("jie", "jie (.), " + Patterns.Int, JumpIfEven);
        matcher.AddPrefix<char, int>("jio", "jio (.), " + Patterns.Int, JumpIfOne); 

        while (Ptr >= 0 && Ptr < Input.Length)
        {
            matcher.Handle(Input[Ptr]);
            Ptr++;
        }

        return Register[Reg('b')];
    }

    public override BigInteger PartTwo()
    {
        Run(PartOne);
        Register[Reg('a')] = 1;
        return PartOne();
    }

    public int Reg(char c) => c - 'a';

    public void Half(char r)
    {
        Register[Reg(r)] /= 2;
    }

    public void Triple(char r)
    {
        Register[Reg(r)] *= 3;
    }

    public void Increment(char r)
    {
        Register[Reg(r)]++;
    }

    public void Jump(int offset)
    {
        Ptr += offset - 1;
    }

    public void JumpIfEven(char r, int offset)
    {
        if (Register[Reg(r)] % 2 == 0) Jump(offset);
    }

    public void JumpIfOne(char r, int offset)
    {
        if (Register[Reg(r)] == 1) Jump(offset);
    }
}

public class Day23_2 : Puzzle<BigInteger>
{
    public Day23_2()
    {
        // Part = 1;
        InputName = "Day23.txt";
    }

    public override BigInteger PartOne()
    {
        var builder = PrefixInstructionBuilder<BigInteger>.Default();
        builder.Add("hlf r", r => r.Value /= 2);
        builder.Add("tpl r", r => r.Value *= 3);
        builder.Add("inc r", r => r.Value++);
        builder.AddCpu("jmp d", (cpu, d) => cpu.JumpRelative(d));
        builder.AddCpu("jie r, d", (cpu, r, d) => r.When(Num.Even, cpu.JumpRelative, d));
        builder.AddCpu("jio r, d", (cpu, r, d) => r.When(1, cpu.JumpRelative, d));

        var cpu = Cpu<BigInteger>.StandardRegisters(2);
        cpu.InstructionSet = builder.BuildAndParseAll(cpu, Input);

        if (Part == 2) cpu.Memory['a'] = 1;
        cpu.Execute();

        return cpu.Memory['b'];
    }
}