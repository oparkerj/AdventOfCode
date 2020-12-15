using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit;

namespace AdventOfCode2020.Puzzles
{
    public class Day10 : Puzzle
    {
        public Day10()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var adapters = Input.Select(int.Parse).ToList();
            adapters.Sort();
            var diff = new int[3];
            var current = 0;
            foreach (var adapter in adapters)
            {
                var d = adapter - current;
                diff[d - 1]++;
                current = adapter;
            }
            diff[2]++;
            WriteLn(diff[0] * diff[2]);
        }

        public override void PartTwo()
        {
            var adapters = Input.Select(int.Parse).ToList();
            var arr = adapters.Append(adapters.Max() + 3).Prepend(0).OrderBy(i => i).ToArray();
            var path = new Dictionary<int, long> {[0] = 1};
            foreach (var i in arr.Skip(1))
            {
                path.TryGetValue(i - 1, out var a);
                path.TryGetValue(i - 2, out var b);
                path.TryGetValue(i - 3, out var c);
                path[i] = a + b + c;
            }
            WriteLn(path[path.Keys.Max()]);
        }
    }
}