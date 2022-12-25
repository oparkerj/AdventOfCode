using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2022.Puzzles;

public class Day25 : Puzzle<string>
{
    public long Parse(string s)
    {
        var value = 0L;
        for (var i = 0; i < s.Length; i++)
        {
            var offset = s.Length - i - 1;
            var place = s[offset] switch
            {
                '2' => 2,
                '1' => 1,
                '0' => 0,
                '-' => -1,
                '=' => -2,
            };
            value += 5L.Pow(i) * place;
        }
        return value;
    }

    public string GetSnafu(long n)
    {
        if (n == 0) return "0";
        var result = "";
        while (n > 0)
        {
            var value = (n % 5 + 2) % 5 - 2;
            var digit = value switch
            {
                2 => '2',
                1 => '1',
                0 => '0',
                -1 => '-',
                -2 => '=',
            };
            result = digit + result;
            n = (n - value) / 5;
        }
        return result;
    }
    
    public override string PartOne()
    {
        return GetSnafu(Input.Select(Parse).Sum());
    }
}