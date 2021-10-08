using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Solvers;
using RegExtract;

namespace AdventOfCode2018.Puzzles
{
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
}