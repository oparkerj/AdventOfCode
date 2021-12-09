using System.Linq;
using AdventToolkit.Collections;
using MoreLinq;

namespace AdventOfCode2021.Puzzles;

public class Day6 : Puzzle
{
    public Day6()
    {
        Part = 2;
    }

    public long Steps(int count)
    {
        var counts = InputLine.Csv().Ints().Frequencies()
            .ToArrayByIndex(9, pair => pair.Key, pair => (long) pair.Value);
            
        var timers = new CircularBuffer<long>(counts);
        for (var i = 0; i < count; i++)
        {
            timers.Offset++;
            timers[6] += timers[8];
        }
        return timers.Unordered().Sum();
    }

    public override void PartOne()
    {
        WriteLn(Steps(80));
    }

    public override void PartTwo()
    {
        WriteLn(Steps(256));
    }
}