using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day23 : Day12
{
    public override void Execute(string inst)
    {
        // Hard code the operations that are done using loops
        if (Ptr == 2)
        {
            // a *= b
            // b--
            // toggle (b * 2)
            Regs[0] *= Regs[1];
            Regs[1]--;
            Regs[2] = Regs[1] * 2;
            Ptr = 15;
        }
        else if (Ptr == 21)
        {
            // a += c * d
            Regs[0] += Regs[2] * Regs[3];
            Ptr = 25;
        }
        else if (inst.StartsWith("tgl"))
        {
            var offset = Val(inst.After(" "));
            var mod = Ptr + (int) offset;
            if (mod < 0 || mod >= Input.Length) return;
            var next = Input[mod];
            if (next.StartsWith("cpy")) Input[mod] = "jnz " + next.After(" ");
            else if (next.StartsWith("inc")) Input[mod] = "dec " + next.After(" ");
            else if (next.StartsWith("dec")) Input[mod] = "inc " + next.After(" ");
            else if (next.StartsWith("jnz")) Input[mod] = "cpy " + next.After(" ");
            else if (next.StartsWith("tgl")) Input[mod] = "inc " + next.After(" ");
        }
        else base.Execute(inst);
    }

    public override void PartOne()
    {
        Regs[Reg('a')] = 7;
        base.PartOne();
    }

    public override void PartTwo()
    {
        // The program computes 12! + 93 * 80
        WriteLn(12.Factorial() + 93 * 80);
    }
}