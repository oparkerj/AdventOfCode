using System.Linq;
using AdventToolkit.Common;

namespace AdventOfCode2021.Puzzles;

public class Day2 : Puzzle
{
    public Day2()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var (x, y) = Input.Select(s =>
        {
            var (dir, amount) = s.SingleSplit(' ');
            return Pos.RelativeDirection(dir[0]) * amount.AsInt();
        }).Sum();
        WriteLn(x * -y);
    }

    public override void PartTwo()
    {
        var p = Pos.Origin;
        var aim = 0;
        foreach (var (dir, amount) in Input.Select(s => s.SingleSplit(' ')))
        {
            var len = amount.AsInt();
            if (dir == "up") aim -= len;
            else if (dir == "down") aim += len;
            else if (dir == "forward")
            {
                p += Pos.Right * len + Pos.Down * (aim * len);
            }
        }
        WriteLn(p.X * -p.Y);
    }
}