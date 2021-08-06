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
                busses.Add((int.Parse(schedule[i]), i));
            }
            // https://mathworld.wolfram.com/ChineseRemainderTheorem.html
            // M is product of moduli
            var M = schedule.Where(s => s != "x").Ints().LongProduct();
            // x = sum(a_i * b_i * M / m_i) (mod M)
            // where b_i * (M / m_i) congruent to 1 (mod m_i)
            // b_i is the modular inverse, calculated using the extended euclidean algorithm.
            var x = busses.Select(bus => (bus.m - bus.a) * (M / bus.m) * (M / bus.m).ModularInverse(bus.m)).Sum();
            x %= M;
            WriteLn(x);
        }
    }
}