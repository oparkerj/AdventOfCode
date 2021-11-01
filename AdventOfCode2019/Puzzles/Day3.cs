using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2019.Puzzles
{
    public class Day3 : Puzzle
    {
        public Day3()
        {
            Part = 2;
        }

        public IEnumerable<Pos> WirePath(string data)
        {
            return data.Csv()
                .Select(Pos.ParseRelative)
                .MakePath(Pos.Origin)
                .ConnectLines();
        }

        public override void PartOne()
        {
            var a = WirePath(Input[0]);
            var b = WirePath(Input[1]);
            var min = a.Intersect(b).Min(pos => pos.MDist(Pos.Origin));
            WriteLn(min);
        }

        public override void PartTwo()
        {
            var a = WirePath(Input[0]).ToArray();
            var b = WirePath(Input[1]).ToArray();
            var aDist = a.Index().Swap().ToDictionaryFirst();
            var bDist = b.Index().Swap().ToDictionaryFirst();
            var min = a.Intersect(b).Min(pos => aDist[pos] + bDist[pos] + 2);
            WriteLn(min);
        }
    }
}