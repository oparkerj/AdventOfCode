using System.Numerics;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day12 : Puzzle
{
    public BigInteger[] Regs;
    public int Ptr;

    public Day12()
    {
        Regs = new BigInteger[4];
    }

    public int Reg(char r) => r - 'a';

    public BigInteger Val(string s) => char.IsLetter(s[0]) ? Regs[Reg(s[0])] : s.AsInt();
    
    public bool IsReg(char c) => char.IsLetter(c);

    public virtual void Execute(string inst)
    {
        var parts = inst.Split(' ');

        if (parts[0] == "cpy" && IsReg(parts[2][0])) Regs[Reg(parts[2][0])] = Val(parts[1]);
        else if (parts[0] == "inc") Regs[Reg(parts[1][0])]++;
        else if (parts[0] == "dec") Regs[Reg(parts[1][0])]--;
        else if (parts[0] == "jnz" && Val(parts[1]) != 0) Ptr += (int) Val(parts[2]) - 1;
    }

    public override void PartOne()
    {
        while (Ptr < Input.Length)
        {
            Execute(Input[Ptr]);
            Ptr++;
        }
        
        WriteLn(Regs[0]);
    }

    public override void PartTwo()
    {
        Regs[Reg('c')] = 1;
        
        PartOne();
    }
}