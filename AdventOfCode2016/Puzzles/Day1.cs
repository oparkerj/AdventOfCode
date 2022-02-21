using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2016.Puzzles;

public class Day1 : Puzzle
{
    public Day1()
    {
        Part = 2;
    }

    public IEnumerable<Pos> GetPoints()
    {
        var dir = Pos.Up;
        var pos = Pos.Origin;
        yield return pos;
        foreach (var move in InputLine.Csv(true))
        {
            dir = move[0] == 'L' ? dir.CounterClockwise() : dir.Clockwise();
            pos += dir * move[1..].AsInt();
            yield return pos;
        }
    }

    public override void PartOne()
    {
        var dest = GetPoints().Last();
        WriteLn(dest.MDist(Pos.Origin));
    }

    public override void PartTwo()
    {
        var firstRepeat = GetPoints().ConnectLines().Prepend(Pos.Origin).FirstRepeat();
        WriteLn(firstRepeat.MDist(Pos.Origin));
    }
}