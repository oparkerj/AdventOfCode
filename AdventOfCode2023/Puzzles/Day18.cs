using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day18 : Puzzle<long>
{
    public long GetArea(IEnumerable<Pos> offsets)
    {
        var total = 0L;
        var x = 0L;
        var perimeter = 0L;
        foreach (var delta in offsets)
        {
            x += delta.X;
            total += x * delta.Y;
            perimeter += delta.MDist(Pos.Zero);
        }
        return Math.Abs(total) + perimeter / 2 + 1;
    }

    public override long PartOne()
    {
        var offsets = Input.Select(s =>
        {
            var (dir, len) = s.Spaced().ToTuple2();
            return Pos.RelativeDirection(dir[0]) * len.AsInt();
        });
        
        return GetArea(offsets);
    }

    public override long PartTwo()
    {
        var offsets = Input.Select(s =>
        {
            var hex = s.Spaced().Last()[2..^1];
            var dir = hex[^1] switch
            {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                '3' => 'U',
            };
            var dist = Convert.ToInt32(hex[..^1], 16);
            return Pos.RelativeDirection(dir) * dist;
        });
        
        return GetArea(offsets);
    }
}