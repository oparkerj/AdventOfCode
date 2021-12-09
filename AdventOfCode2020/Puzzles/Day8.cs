using System.Collections.Generic;
using AdventToolkit;

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