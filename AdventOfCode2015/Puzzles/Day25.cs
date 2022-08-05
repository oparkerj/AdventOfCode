using System.Numerics;
using AdventToolkit;
using AdventToolkit.Common;

namespace AdventOfCode2015.Puzzles;

public class Day25 : Puzzle
{
    public override void PartOne()
    {
        BigInteger current = 20151125;
        Pos pos = (1, 1);

        BigInteger mul = 252533;
        BigInteger mod = 33554393;

        while (pos is not (3029, 2947))
        {
            pos = pos.Y == 1 ? (1, pos.X + 1) : (pos.X + 1, pos.Y - 1);
            current = current * mul % mod;
        }
        
        WriteLn(current);
    }
}