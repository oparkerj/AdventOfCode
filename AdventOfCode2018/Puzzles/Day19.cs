using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2018.Puzzles;

public class Day19 : Puzzle
{
    public Day19()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var ops = new Day16().Ops;
            
        var ptr = 0;
        var bind = InputLine[4..].AsInt();
        var reg = new int[6];
        var prgm = Input.Skip(1).Extract<(string Op, int A, int B, int C)>(@"(\S+) (\d+) (\d+) (\d+)").ToArray();

        while (ptr >= 0 && ptr < prgm.Length)
        {
            var (op, a, b, c) = prgm[ptr];
            reg[bind] = ptr;
            reg[c] = ops[op](reg, a, b);
            ptr = reg[bind];
            ptr++;
        }
        WriteLn(reg[0]);
    }

    public override void PartTwo()
    {
        var reg = new int[6];
        // Program setup after reg[0] = 1
        reg[2] = 10551298;
        reg[5] = 10550400;

        // Translation of instructions:
        // reg[1] = 1;
        // do
        // {
        //     reg[3] = 1;
        //     do
        //     {
        //         if (reg[1] * reg[3] == reg[2])
        //         {
        //             reg[0] += reg[1];
        //         }
        //         reg[3] += 1;
        //     } while (reg[3] <= reg[2]);
        //     reg[1] += 1;
        // } while (reg[1] <= reg[2]);

        // Result: Sum of factors of reg[2]
        var d = reg[2];
        var result = Interval.RangeInclusive(1, d).Where(i => d % i == 0).Sum();
        WriteLn(result);
    }
}