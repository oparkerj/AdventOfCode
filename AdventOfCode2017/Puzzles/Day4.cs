using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles;

public class Day4 : Puzzle
{
    public Day4()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var result = Input.Count(s => s.Split(' ').FirstRepeat() == null);
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var result = Input.Count(s => s.Split(' ').Select(w => w.Sorted().Str()).FirstRepeat() == null);
        WriteLn(result);
    }
}