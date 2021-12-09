using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2017.Puzzles;

public class Day8 : Puzzle
{
    public Day8()
    {
        Part = 2;
    }

    private IEnumerable<Inst> GetInput() => Input.Extract<Inst>(@"(\w+) (\w+) (-?\d+) if (\w+) (\S+) (-?\d+)");

    public override void PartOne()
    {
        var reg = new DefaultDict<string, int>();
        foreach (var (r, sign, offset, cmp, op, value) in GetInput())
        {
            if (op.Operation(reg[cmp], value))
            {
                reg[r] += sign.Inc ? offset : -offset;
            }
        }
        WriteLn(reg.Values.Max());
    }

    public override void PartTwo()
    {
        var reg = new DefaultDict<string, int>();
        var max = 0;
        foreach (var (r, sign, offset, cmp, op, value) in GetInput())
        {
            if (op.Operation(reg[cmp], value))
            {
                reg[r] += sign.Inc ? offset : -offset;
                max = Math.Max(max, reg[r]);
            }
        }
        WriteLn(max);
    }

    private record Inst(string Reg, Sign Sign, int Offset, string Cmp, Op Op, int Value);

    private readonly struct Sign
    {
        public readonly bool Inc;

        public Sign(bool inc) => Inc = inc;

        public static Sign Parse(string s) => new(s == "inc");
    }

    private readonly struct Op
    {
        public readonly Func<int, int, bool> Operation;

        public Op(Func<int, int, bool> operation) => Operation = operation;

        public static Op Parse(string s)
        {
            return s switch
            {
                ">" => new Op(Num.Gt),
                "<" => new Op(Num.Lt),
                ">=" => new Op(Num.Ge),
                "<=" => new Op(Num.Le),
                "==" => new Op(Num.Eq),
                "!=" => new Op(Num.Neq),
                _ => throw new Exception()
            };
        }
    }
}