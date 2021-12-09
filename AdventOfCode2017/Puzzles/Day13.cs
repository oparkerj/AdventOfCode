using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles;

public class Day13 : Puzzle
{
    public Day13()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var layers = Input.ReadKeys(int.Parse, int.Parse);
        var result = layers.Where(pair => pair.Key % ((pair.Value - 1) * 2) == 0)
            .Select(pair => pair.Key * pair.Value)
            .Sum();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var layers = Input.ReadKeys(int.Parse, int.Parse);
        var delay = 0;
        while (true)
        {
            if (layers.All(pair => (pair.Key + delay) % ((pair.Value - 1) * 2) != 0)) break;
            delay++;
        }
        WriteLn(delay);
    }
}