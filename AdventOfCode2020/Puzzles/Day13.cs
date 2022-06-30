using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles;

public class Day13 : Puzzle
{
    public Day13()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var time = int.Parse(Input[0]);
        var id = Input[1].Csv()
            .Where(s => s != "x")
            .Ints()
            .Select(i => (i, i - time % i))
            .OrderBy(tuple => tuple.Item2)
            .Select(tuple => tuple.i * tuple.Item2)
            .First();
        WriteLn(id);
    }

    public override void PartTwo()
    {
        var busses = new List<(int m, int a)>();
        var schedule = Input[1].Csv().ToArray();
        for (var i = 0; i < schedule.Length; i++)
        {
            if (schedule[i] == "x") continue;
            var mi = int.Parse(schedule[i]);
            busses.Add((mi, mi - i));
        }

        var (m, a) = busses.Separate();
        var x = a.ChineseRemainder(m);
        WriteLn(x);
    }
}