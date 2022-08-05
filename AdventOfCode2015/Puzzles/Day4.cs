using System.Security.Cryptography;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day4 : Puzzle<int>
{
    public override int PartOne()
    {
        using var md5 = MD5.Create();
        var key = InputLine;
        return Num.Positive().First(i => (key + i).Hash(md5).StartsWith("00000"));
    }

    public override int PartTwo()
    {
        using var md5 = MD5.Create();
        var key = InputLine;
        return Num.Positive().First(i => (key + i).Hash(md5).StartsWith("000000"));
    }
}