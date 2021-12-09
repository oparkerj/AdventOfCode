using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles;

public class Day11 : Puzzle
{
    public Day11()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var target = InputLine.Csv()
            .Select(Pos3D.ParseHexDir)
            .Aggregate(Pos3D.Add);
        var result = target.HexDistance(Pos3D.Origin);
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var result = InputLine.Csv()
            .Select(Pos3D.ParseHexDir)
            .Scan(Pos3D.Add)
            .Select(p => p.HexDistance(Pos3D.Origin))
            .Max();
        WriteLn(result);
    }
}