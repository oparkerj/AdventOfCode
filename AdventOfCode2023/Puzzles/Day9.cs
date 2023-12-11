using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day9 : Puzzle<int>
{
    public int Extrapolate(List<int> seq)
    {
        var buf = new List<List<int>> {seq};
        while (buf[^1].Any(i => i != 0))
        {
            buf.Add(buf[^1].Pairwise((a, b) => b - a).ToList());
        }

        var end = 0;
        var current = buf.Count - 1;
        while (--current >= 0)
        {
            var (index, value) = Part == 2 ? (0, -end) : (^1, end);
            end = buf[current][index] + value;
        }
        return end;
    }
    
    public override int PartOne()
    {
        return Input.Select(s => s.Spaced().Ints().ToList())
            .Select(Extrapolate)
            .Sum();
    }
}