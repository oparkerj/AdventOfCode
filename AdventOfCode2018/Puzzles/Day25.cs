using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles;

public class Day25 : Puzzle
{
    public List<Pos4D> ReadPoints() => Input.Select(Pos4D.Parse).ToList();
        
    public override void PartOne()
    {
        var count = ReadPoints().GroupPairs((a, b) => a.MDist(b) <= 3).Count;
        WriteLn(count);
    }
}