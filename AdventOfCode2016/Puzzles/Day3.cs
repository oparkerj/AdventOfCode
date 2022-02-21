using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day3 : Puzzle
{
    public Day3()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var result = Input.Extract<Triangle>(@"\s*(\d+)\s+(\d+)\s+(\d+)\s*").Count(triangle => triangle.IsPossible);
        WriteLn(result);
    }
    
    public override void PartTwo()
    {
        var count = Input.Select(s => s.Spaced())
            .Chunk(3)
            .Select(s => Enumerable.Range(0, 3).Select(i => s.Select(r => r[i].AsInt()).ToArray(3)))
            .Flatten()
            .Select(ints => new Triangle(ints[0], ints[1], ints[2]))
            .Count(triangle => triangle.IsPossible);
        WriteLn(count);
    }

    public record Triangle(int A, int B, int C)
    {
        public bool IsPossible => A + B > C && B + C > A && A + C > B;
    }
}