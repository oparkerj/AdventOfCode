using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using AdventToolkit.Utilities.Computer.Builders.Opcode;
using AdventToolkit.Utilities.Computer.Core;
using AdventToolkit.Utilities.Computer.Memory;
using RegExtract;

namespace AdventOfCode2018.Puzzles;

public class Day16 : Puzzle
{
    public readonly Dictionary<string, Func<int[], int, int, int>> Ops = new();
    public readonly Dictionary<int, string> OpLookup = new();

    public Day16()
    {
        DefineOps();
        Part = 2;
    }

    public (int Op, int A, int B, int C) ReadRegisters(string line)
    {
        return line.Extract<(int, int, int, int)>(@"\D*(\d+)\D+(\d+)\D+(\d+)\D+(\d+)\D*");
    }
        
    public List<Sample> ReadSamples()
    {
        var result = new List<Sample>();
        foreach (var sample in AllGroups[..^1])
        {
            result.Add(new Sample(
                ReadRegisters(sample[0]),
                ReadRegisters(sample[1]),
                ReadRegisters(sample[2])
            ));
        }
        return result;
    }

    public int[] ToReg((int, int, int, int) tuple)
    {
        var reg = new[] {tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4};
        return reg;
    }

    public void DefineOps()
    {
        Ops["addr"] = (reg, a, b) => reg[a] + reg[b];
        Ops["addi"] = (reg, a, b) => reg[a] + b;
        Ops["mulr"] = (reg, a, b) => reg[a] * reg[b];
        Ops["muli"] = (reg, a, b) => reg[a] * b;
        Ops["banr"] = (reg, a, b) => reg[a] & reg[b];
        Ops["bani"] = (reg, a, b) => reg[a] & b;
        Ops["borr"] = (reg, a, b) => reg[a] | reg[b];
        Ops["bori"] = (reg, a, b) => reg[a] | b;
        Ops["setr"] = (reg, a, b) => reg[a];
        Ops["seti"] = (reg, a, b) => a;
        Ops["gtir"] = (reg, a, b) => (a > reg[b]).AsInt();
        Ops["gtri"] = (reg, a, b) => (reg[a] > b).AsInt();
        Ops["gtrr"] = (reg, a, b) => (reg[a] > reg[b]).AsInt();
        Ops["eqir"] = (reg, a, b) => (a == reg[b]).AsInt();
        Ops["eqri"] = (reg, a, b) => (reg[a] == b).AsInt();
        Ops["eqrr"] = (reg, a, b) => (reg[a] == reg[b]).AsInt();
    }

    public bool TestSample(Sample sample, Func<int[], int, int, int> inst)
    {
        var reg = ToReg(sample.Before);
        reg[sample.Inst.C] = inst(reg, sample.Inst.A, sample.Inst.B);
        return reg.SequenceEqual(ToReg(sample.After));
    }

    public void ExecuteOp((int Op, int A, int B, int C) inst, int[] reg)
    {
        var result = Ops[OpLookup[inst.Op]](reg, inst.A, inst.B);
        reg[inst.C] = result;
    }

    public override void PartOne()
    {
        var count = ReadSamples().Select(sample =>
        {
            return Ops.Count(pair => TestSample(sample, pair.Value));
        }).Count(i => i >= 3);
        WriteLn(count);
    }

    public override void PartTwo()
    {
        var samples = ReadSamples();
        var possible = new OneToOne<int, string>();
        possible.AddKeys(samples.Select(sample => sample.Inst.Op));
        possible.AddValues(Ops.Keys);
        possible.ReduceWithValid(i => samples.Where(sample => sample.Inst.Op == i), (op, sample, eliminate) =>
        {
            foreach (var pair in Ops.Where(pair => !TestSample(sample, pair.Value)))
            {
                eliminate(pair.Key);
            }
        });
        possible.ReduceToSingles();
        foreach (var (key, inst) in possible.Results)
        {
            OpLookup[key] = inst;
        }

        var reg = new int[4];
        foreach (var inst in AllGroups[^1].Select(ReadRegisters))
        {
            ExecuteOp(inst, reg);
        }
        WriteLn(reg[0]);
    }

    public record Sample((int Op, int A, int B, int C) Before,
        (int Op, int A, int B, int C) Inst,
        (int Op, int A, int B, int C) After);
}

public class Day16_2 : Improve<Day16, int>
{
    public Cpu<int> Cpu;
    public OpInstructionBuilder<int, bool> Builder;
    public OneToOne<int, int> Possible;

    public Day16_2()
    {
        Setup();
        Possible = new OneToOne<int, int>();
    }

    public void Setup()
    {
        Cpu = Cpu<int>.StandardRegisters(4);
        Builder = PrefixInstructionBuilder<int>.Default();
        Builder.Add("addr r r r", (a, b, c) => c.Value = a.Value + b.Value);
        Builder.Add("addi r d r", (a, b, c) => c.Value = a.Value + b.Value);
        Builder.Add("mulr r r r", (a, b, c) => c.Value = a.Value * b.Value);
        Builder.Add("muli r d r", (a, b, c) => c.Value = a.Value * b.Value);
        Builder.Add("banr r r r", (a, b, c) => c.Value = a.Value & b.Value);
        Builder.Add("bani r d r", (a, b, c) => c.Value = a.Value & b.Value);
        Builder.Add("borr r r r", (a, b, c) => c.Value = a.Value | b.Value);
        Builder.Add("bori r d r", (a, b, c) => c.Value = a.Value | b.Value);
        Builder.Add("setr r d r", (a, _, c) => c.Value = a.Value);
        Builder.Add("seti d d r", (a, _, c) => c.Value = a.Value);
        Builder.Add("gtir d r r", (a, b, c) => c.Value = (a.Value > b.Value).AsInt());
        Builder.Add("gtri r d r", (a, b, c) => c.Value = (a.Value > b.Value).AsInt());
        Builder.Add("gtrr r r r", (a, b, c) => c.Value = (a.Value > b.Value).AsInt());
        Builder.Add("eqir d r r", (a, b, c) => c.Value = (a.Value == b.Value).AsInt());
        Builder.Add("eqri r d r", (a, b, c) => c.Value = (a.Value == b.Value).AsInt());
        Builder.Add("eqrr r r r", (a, b, c) => c.Value = (a.Value == b.Value).AsInt());
        Cpu.InstructionSet = Builder.BuildInstructionSet();
    }

    public override int PartOne()
    {
        var actions = Builder.GetInstructionHandler(Cpu).OpActions;
        var registers = (Registers<int>) Cpu.Memory;

        // Possible is an object responsible for figuring out the mapping between
        // instruction numbers and the index of the actual instruction.
        // Add all instruction numbers.
        Possible.AddKeys(AllGroups[..^1].SelectIndex(1).Extract<int>(Patterns.Int));
        // Add all instruction indices.
        Possible.AddValues(Enumerable.Range(0, actions.Length));
        
        // Test all samples
        return AllGroups[..^1].Count(sample =>
        {
            var state = sample.Extract<List<int>>(Patterns.IntList).ToArray();
            var match = actions.Select((action, i) =>
                {
                    // Set up registers
                    registers.CopyIn(state[0]);
                    // Test instruction
                    var inst = Builder.ParseAsOp(Cpu, sample[1], i);
                    action(Cpu, inst);
                    // Check registers after
                    return registers.Storage.SequenceEqual(state[2]) ? i : -1;
                })
                .Where(i => i >= 0)
                .ToList();
            // We know which instructions worked for this instruction number.
            // Use that to eliminate some of the possibilities.
            Possible.ReduceWithValid(state[1][0], match);
            return match.Count >= 3;
        });
    }

    public override int PartTwo()
    {
        PartOne();
        Cpu.Memory.Reset();
        
        Possible.ReduceToSingles();
        var realOp = Possible.Mappings();

        var code = AllGroups[^1]
            .Select(s => Builder.ParseAsOp(Cpu, s, realOp[s.TakeInt()]))
            .ToArray();
        Builder.GetInstructionSet(Cpu).Instructions = code;
        
        Cpu.Execute();
        return Cpu.Memory[0];
    }
}