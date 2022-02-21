using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2016.Puzzles;

public class Day2 : Puzzle
{
    public Day2()
    {
        Part = 2;
    }

    public int GetNumber(Pos pos)
    {
        return -3 * pos.Y + 4 + (pos.X + 1);
    }
    
    public override void PartOne()
    {
        var keypad = new Rect(-1, -1, 3, 3);
        var code = Input.Scan(Pos.Origin, (pos, line) => line.Select(Pos.RelativeDirection).Aggregate(pos, (a, b) => a.SumWithin(b, keypad)))
            .Skip(1).Select(GetNumber).Str();
        WriteLn(code);
    }

    private Pos LimitedSum(Pos a, Pos b, int dist)
    {
        var target = a + b;
        return target.MDist(Pos.Origin) > dist ? a : target;
    }

    private char DiamondNumber(Pos p)
    {
        return p switch
        {
            (0, 2) => '1',
            (-1, 1) => '2',
            (0, 1) => '3',
            (1, 1) => '4',
            (-2, 0) => '5',
            (-1, 0) => '6',
            (0, 0) => '7',
            (1, 0) => '8',
            (2, 0) => '9',
            (-1, -1) => 'A',
            (0, -1) => 'B',
            (1, -1) => 'C',
            (0, -2) => 'D',
            _ => ' '
        };
    }

    public override void PartTwo()
    {
        var code = Input.Scan(new Pos(-2, 0), (pos, line) => line.Select(Pos.RelativeDirection).Aggregate(pos, (a, b) => LimitedSum(a, b, 2)))
            .Skip(1).Select(DiamondNumber).Str();
        WriteLn(code);
    }
}