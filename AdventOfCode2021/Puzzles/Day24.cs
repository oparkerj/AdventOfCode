using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;

namespace AdventOfCode2021.Puzzles;

public class Day24 : Puzzle
{
    public Day24()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var checks = Input.Chunk(Input.Length / 14).Select(part =>
        {
            var a = part[4].Split(' ')[^1].AsInt();
            var b = part[5].Split(' ')[^1].AsInt();
            var c = part[15].Split(' ')[^1].AsInt();
            return (a, b, c);
        }).ToList();

        var result = new int[14];
        var stack = new Stack<(int i, int c)>();
        var range = new Interval(1, 9);

        for (var i = 0; i < 14; i++)
        {
            var (a, b, c) = checks[i];
            if (a == 1) stack.Push((i, c));
            else
            {
                var (oi, oc) = stack.Pop();
                var possible = range.Shift(oc + b).Overlap(range);
                var current = Part == 1 ? possible.Last : possible.Start;
                var other = current - (oc + b);
                result[i] = current;
                result[oi] = other;
            }
        }

        WriteLn(result.Str());
    }
}