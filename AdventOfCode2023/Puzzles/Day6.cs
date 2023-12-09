using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day6 : Puzzle<int, long>
{
    public override int PartOne()
    {
        var times = Input[0].After(':').Spaced().Ints().ToList();
        var dists = Input[1].After(':').Spaced().Ints().ToList();

        var total = 1;
        for (var i = 0; i < times.Count; i++)
        {
            var wins = 0;
            var (time, dist) = (times[i], dists[i]);
            for (var h = 0; h < time; h++)
            {
                if (h * time - h > dist)
                {
                    wins++;
                }
            }
            total *= wins;
        }
        return total;
    }

    public override long PartTwo()
    {
        var time = Input[0].After(':').Spaced().Str().AsLong();
        var distance = Input[1].After(':').Spaced().Str().AsLong();

        // Quadratic formula
        var a = -1;
        var b = time;
        var c = -distance;
        var first = (-b - (b * b - 4 * a * c).SqrtFloor()) / (2 * a);
        var second = (-b + (b * b - 4 * a * c).SqrtFloor()) / (2 * a);
        return Math.Abs(first - second) + 1;
    }
}