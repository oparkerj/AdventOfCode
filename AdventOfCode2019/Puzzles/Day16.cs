using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles;

public class Day16 : Puzzle
{
    public int[] Code;

    public Day16()
    {
        Part = 2;
    }

    public IEnumerable<int> PatternFor(int index)
    {
        var count = index + 1;
        for (var i = 0; i < count - 1; i++)
        {
            yield return 0;
        }
        while (true)
        {
            for (var i = 0; i < count; i++) yield return 1;
            for (var i = 0; i < count; i++) yield return 0;
            for (var i = 0; i < count; i++) yield return -1;
            for (var i = 0; i < count; i++) yield return 0;
        }
    }

    public void Fft()
    {
        for (var i = 0; i < Code.Length; i++)
        {
            Code[i] = Code.ZipShortest(PatternFor(i), (a, b) => a * b).Sum().Digits().First();
        }
    }

    public override void PartOne()
    {
        Code = InputLine.Ints().ToArray();
        for (var i = 0; i < 100; i++)
        {
            Fft();
        }
        WriteLn(Code.Take(8).Str());
    }

    public override void PartTwo()
    {
        Code = Enumerable.Repeat(InputLine, 10000).Str().Ints().ToArray();
        var offset = Code.Take(7).AsInt();
        for (var i = 0; i < 100; i++)
        {
            var sum = Enumerable.Range(offset, Code.Length - offset).Select(index => Code[index]).Sum();
            for (var j = offset; j < Code.Length; j++)
            {
                var oldSum = sum;
                sum -= Code[j];
                Code[j] = oldSum % 10;
            }
        }
        WriteLn(Code.Skip(offset).Take(8).Str());
    }
}