using System.Linq;
using AdventToolkit.Collections;

namespace AdventOfCode2021.Puzzles;

public class Day3 : Puzzle
{
    public Day3()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var len = Input.Length / 2;
        var result = 0;
        for (var i = 0; i < 12; i++)
        {
            var ones = Input.Select(s => s[i].AsInt()).Sum();
            result |= (ones > len / 2).AsInt() << (11 - i);
        }
        WriteLn(result * (~result & 0xFFF));
    }

    public override void PartTwo()
    {
        var gen = Input.ToList();
        var i = 0;
        while (gen.Count > 1)
        {
            var ones = gen.Select(s => s[i].AsInt()).Sum();
            var most = !new Interval(gen.Count).InLowerHalf(ones);
            gen.RemoveAll(s => s[i] == '1' != most);
            i++;
        }
            
        var o2 = Input.ToList();
        i = 0;
        while (o2.Count > 1)
        {
            var ones = o2.Select(s => s[i].AsInt()).Sum();
            var least = new Interval(o2.Count).InLowerHalf(ones);
            o2.RemoveAll(s => s[i] == '1' != least);
            i++;
        }
            
        WriteLn(gen[0].BinaryInt() * o2[0].BinaryInt());
    }
}