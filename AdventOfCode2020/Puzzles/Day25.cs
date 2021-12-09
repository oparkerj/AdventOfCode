using AdventToolkit;

namespace AdventOfCode2020.Puzzles;

public class Day25 : Puzzle
{
    public const int Mod = 20201227;

    public long Transform(int i, int loopSize)
    {
        var value = 1L;
        for (var k = 0; k < loopSize; k++)
        {
            value = value * i % Mod;
        }
        return value;
    }

    public int GetLoopNumber(int target)
    {
        var count = 0;
        var current = 1;
        while (true)
        {
            count++;
            if ((current = current * 7 % Mod) == target) break;
        }
        return count;
    }

    public override void PartOne()
    {
        var target = int.Parse(Input[0]);
        var loopNumber = GetLoopNumber(target);
        WriteLn(loopNumber);
        var result = Transform(int.Parse(Input[1]), loopNumber);
        WriteLn(result);
    }
}