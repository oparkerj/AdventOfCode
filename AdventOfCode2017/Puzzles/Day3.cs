using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles
{
    public class Day3 : Puzzle
    {
        public Day3()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var input = InputInt;
            var layer = (int) Math.Ceiling(Math.Sqrt(input)) / 2;
            var factor = layer * 2 + 1;
            var max = factor * factor;
            var min = (factor - 2) * (factor - 2) + 1;
            var len = (max - min + 1) / 4;
            var cornerDist = len / 2 - Math.Abs((input - min) % len - (len / 2 - 1));
            var result = layer * 2 - cornerDist;
            WriteLn(result);
        }

        public IEnumerable<Pos> Spiral()
        {
            yield return Pos.Origin;

            var dirs = new[] {Pos.Up, Pos.Left, Pos.Down, Pos.Right};
            var delta = new Pos(1, -1);
            var last = delta;
            var len = 2;
            while (true)
            {
                var shellLen = len;
                var shell = dirs.Scan(last, (a, b) => a + b * shellLen)
                    .Pairwise(PosExtensions.EachTo)
                    .Flatten();
                foreach (var pos in shell)
                {
                    yield return pos;
                }
                last += delta;
                len += 2;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public override void PartTwo()
        {
            var input = InputInt;
            var grid = new Grid<int>(true);
            grid[Pos.Origin] = 1;
            foreach (var pos in Spiral().Skip(1))
            {
                var value = pos.NeighborValues(grid).Sum();
                if (value > input)
                {
                    WriteLn(value);
                    break;
                }
                grid[pos] = value;
            }
        }
    }
}