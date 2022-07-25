using System.Security.Cryptography;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day4 : Puzzle
{
    public override void PartOne()
    {
        using var md5 = MD5.Create();
        var key = InputLine;
        var result = Num.Positive().First(i => (key + i).Hash(md5).StartsWith("00000"));
        WriteLn(result);
    }

    public override void PartTwo()
    {
        using var md5 = MD5.Create();
        var key = InputLine;
        var result = Num.Positive().First(i => (key + i).Hash(md5).StartsWith("000000"));
        WriteLn(result);
    }
}