using System.Collections.Generic;
using AdventToolkit;
using AdventToolkit.Utilities.Computer;

namespace AdventOfCode2020.Puzzles;

public class Day8 : Puzzle
{
    public Day8()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var run = new HashSet<int>();
        var c = new Comp(Input);
        while (!run.Contains(c.Pointer))
        {
            run.Add(c.Pointer);
            c.Step();
        }
        WriteLn(c.Accumulator);
    }

    public bool Try(Comp c, out int acc)
    {
        var run = new HashSet<int>();
        while (!run.Contains(c.Pointer) && !c.Done)
        {
            run.Add(c.Pointer);
            c.Step();
        }
        acc = c.Accumulator;
        return c.Done;
    }

    public override void PartTwo()
    {
        var c = new Comp(Input);
        for (var i = 0; i < c.Length; i++)
        {
            c.Reset();
            var inst = c[i];
            var old = inst.Op;
            if (inst.Op == Comp.Op.Jmp) inst.Op = Comp.Op.Nop;
            else if (inst.Op == Comp.Op.Nop) inst.Op = Comp.Op.Jmp;
            else continue;
            if (Try(c, out var acc))
            {
                WriteLn(acc);
                break;
            }
            inst.Op = old;
        }
    }
}

public class Day8_2 : Puzzle<int>
{
    public Cpu<int> Cpu;
    public int Accumulator;
    public HashSet<int> Executed;

    public Day8_2()
    {
        InputName = CopyInput<Day8>();
    }

    public PrefixInstructionBuilder<int> Setup()
    {
        var builder = PrefixInstructionBuilder<int>.Default();
        builder.Add("acc d", v => Accumulator += v);
        builder.AddCpu("jmp d", (cpu, v) => cpu.JumpRelative(v));
        builder.Add("nop d");
        
        Cpu = Cpu<int>.Standard();
        Cpu.InstructionSet = builder.BuildAndParseAll(Cpu, Input);
        Executed = new HashSet<int>();
        // Add a pipeline filter which will stop execution if the same instruction
        // is executed twice.
        Cpu.Pipeline = new PipelineFilter<int>(Cpu.Pipeline, cpu => Executed.Add(cpu.Pointer));
        
        return builder;
    }

    public override int PartOne()
    {
        Setup();
        Cpu.Execute();
        return Accumulator;
    }

    public override int PartTwo()
    {
        var builder = Setup();

        var jmp = builder.OpIndex("jmp");
        var nop = builder.OpIndex("nop");
        var instructions = builder.GetInstructionSet(Cpu).Instructions;
        foreach (var inst in instructions)
        {
            if (inst.Opcode == jmp) inst.Opcode = nop;
            else if (inst.Opcode == nop) inst.Opcode = jmp;
            else continue;

            Accumulator = 0;
            Cpu.Pointer = 0;
            Executed.Clear();
            Cpu.Execute();
            if (Cpu.Pointer == instructions.Length) return Accumulator;

            inst.Opcode = inst.Opcode == jmp ? nop : jmp;
        }

        WriteLn("Not found");
        return 0;
    }
}