using System.Collections.Generic;
using AdventToolkit;

namespace AdventOfCode2018.Puzzles;

public class Day21 : Puzzle
{
    public Day21()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var part = Part;
        var seen = new HashSet<int>();
        int recent = 0;
        var reg = new int[6];
            
        Part2:
        reg[3] = reg[4] | 65536;
        reg[4] = 2176960;
        bool b1;
        do {
            reg[2] = reg[3] & 255;
            reg[4] += reg[2];
            reg[4] &= 16777215;
            reg[4] *= 65899;
            reg[4] &= 16777215;
            b1 = false;
            if (256 > reg[3]) continue;
            var i = 1;
            while (i * 256 <= reg[3]) i++;
            reg[3] = i - 1;
            b1 = true;
        } while (b1);
        if (seen.Contains(reg[4])) {
            goto Result;
        }
        seen.Add(recent = reg[4]);
        if (part == 2 && reg[4] != reg[0]) goto Part2;
            
        Result:
        WriteLn(recent);
    }
}