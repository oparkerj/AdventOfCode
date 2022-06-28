using System.Security.Cryptography;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day5 : Puzzle
{
    public Day5()
    {
        Part = 1;
    }

    public override void PartOne()
    {
        var password = "";
        var search = 0;

        var key = InputLine;
        using var md5 = MD5.Create();

        while (password.Length < 8)
        {
            var hash = (key + search).Hash(md5);
            if (hash.StartsWith("00000")) password += hash[5];
            search++;
        }

        WriteLn(password);
    }

    public override void PartTwo()
    {
        var password = new char[8];
        var found = 0;
        var search = 0;

        var key = InputLine;
        using var md5 = MD5.Create();
        
        while (found < 8)
        {
            var hash = (key + search).Hash(md5);
            var pos = hash[5].AsInt();
            if (hash.StartsWith("00000") && pos < 8 && password[pos] == '\0')
            {
                password[pos] = hash[6];
                found++;
            }
            search++;
        }
        
        WriteLn(password.Str());
    }
}