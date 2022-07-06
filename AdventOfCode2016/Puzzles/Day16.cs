using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day16 : Puzzle
{
    public int Length = 272;

    public bool GetValue(int index)
    {
        var len = InputLine.Length;
        var effectiveLength = len;

        while (effectiveLength <= index)
        {
            effectiveLength = effectiveLength * 2 + 1;
        }

        var flip = false;
        while (effectiveLength > len)
        {
            var half = effectiveLength / 2;
            if (index == half) return flip;
            if (index > half)
            {
                index = effectiveLength - index - 1;
                flip = !flip;
            }
            effectiveLength /= 2;
        }

        return (InputLine[index] == '1') ^ flip;
    }

    public IEnumerable<bool> Checksum(IEnumerable<bool> data)
    {
        var checksum = data.ToList();
        while (checksum.Count % 2 == 0)
        {
            for (var i = 0; i < checksum.Count; i += 2)
            {
                checksum[i / 2] = checksum[i] == checksum[i + 1];
            }
            checksum.RemoveRange(checksum.Count / 2, checksum.Count / 2);
        }
        return checksum;
    }

    public override void PartOne()
    {
        var data = Enumerable.Range(0, Length).Select(GetValue);
        var checksum = Checksum(data);
        WriteLn(checksum.AsInts().Str());
    }

    public override void PartTwo()
    {
        Length = 35651584;
        PartOne();
    }
}