using System.Collections.Generic;
using System.Linq;
using AdventOfCode2019.IntCode;
using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day7 : Puzzle
    {
        public Day7()
        {
            Part = 2;
        }

        public long GetSignal(IList<long> inputs)
        {
            var network = new Network();
            network.AddSeries(InputLine, 5);
            network.WithSetup(Network.Insert(inputs), Network.Insert(0, 0));
            return network.RunSeries();
        }
        
        public override void PartOne()
        {
            var max = Enumerable.Range(0, 5).Longs().Permutations().Max(GetSignal);
            WriteLn(max);
        }

        public long GetSignal2(IList<long> inputs)
        {
            var network = new Network();
            network.AddLoop(InputLine, 5);
            network.WithSetup(Network.Insert(inputs), Network.Insert(0, 0));
            return network.RunLoopAsync().GetAwaiter().GetResult();
        }

        public override void PartTwo()
        {
            var max = Enumerable.Range(5, 5).Longs().Permutations().Max(GetSignal2);
            WriteLn(max);
        }
    }
}