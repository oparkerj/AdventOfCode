using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day13 : Puzzle
    {
        public Day13()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var time = int.Parse(Input[0]);
            var id = Input[1].Split(',')
                .Where(s => s != "x")
                .Select(int.Parse)
                .Select(i => (i, i - time % i))
                .OrderBy(tuple => tuple.Item2)
                .Select(tuple => tuple.i * tuple.Item2)
                .First();
            WriteLn(id);
        }

        public override void PartTwo()
        {
            var busses = new List<(int id, int i)>();
            var schedule = Input[1].Split(',').ToArray();
            for (var i = 0; i < schedule.Length; i++)
            {
                if (schedule[i] == "x") continue;
                busses.Add((int.Parse(schedule[i]), i));
            }
            var m = schedule.Where(s => s != "x").Ints().LongProduct();
            var x = busses.Select(bus => (bus.id - bus.i) * (m / bus.id) * (m / bus.id).ModularInverse(bus.id)).Sum();
            x %= m;
            WriteLn(x);
        }
    }
}