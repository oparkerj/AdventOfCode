using System.Numerics;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day12 : Puzzle
{
    private BigInteger[] _regs;
    private int _ptr;

    public Day12()
    {
        _regs = new BigInteger[4];
    }

    public int Reg(char r) => r - 'a';

    public BigInteger Val(string s) => char.IsLetter(s[0]) ? _regs[Reg(s[0])] : s.AsInt();

    public void Execute(string inst)
    {
        var parts = inst.Split(' ');

        if (parts[0] == "cpy") _regs[Reg(parts[2][0])] = Val(parts[1]);
        else if (parts[0] == "inc") _regs[Reg(parts[1][0])]++;
        else if (parts[0] == "dec") _regs[Reg(parts[1][0])]--;
        else if (parts[0] == "jnz" && Val(parts[1]) != 0) _ptr += parts[2].AsInt() - 1;
    }

    public override void PartOne()
    {
        while (_ptr < Input.Length)
        {
            Execute(Input[_ptr]);
            _ptr++;
        }
        
        WriteLn(_regs[0]);
    }

    public override void PartTwo()
    {
        _regs[Reg('c')] = 1;
        
        PartOne();
    }
}