using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2018.Puzzles;

public class Day11 : Puzzle
{
    public readonly int Serial;

    public Day11()
    {
        Serial = InputLine.AsInt();
        Part = 2;
    }

    public int PowerLevel(Pos pos)
    {
        var rack = pos.X + 10;
        var power = (rack * pos.Y + Serial) * rack;
        return power.Digits().ElementAtOrDefault(2) - 5;
    }

    public Grid<int> BuildTable()
    {
        var grid = new Grid<int>();
        grid.ApplyValues(new Rect((1, 1), (300, 300)), PowerLevel);
        grid.BuildSummedArea();
        return grid;
    }

    public override void PartOne()
    {
        var table = BuildTable();
        var result = Algorithms.Sequences(2, 300 - 2)
            .ToPositions()
            .SelectMaxBy(pos => table.GetSummedArea(new Rect(pos, 3, 3)));
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var table = BuildTable();
        var result = Enumerable.Range(1, 300).SelectMany(i => Algorithms.Sequences(2, 300 - i + 1).ToPositions3D(i))
            .SelectMaxBy(square => table.GetSummedArea(new Rect(square.To2D, square.Z, square.Z)));
        WriteLn(result);
    }
}