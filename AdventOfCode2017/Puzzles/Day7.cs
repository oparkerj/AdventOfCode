using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2017.Puzzles
{
    public class Day7 : Puzzle
    {
        public Day7()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var tree = Input.ToWeightedTree(@"(?<Value>.+) \((?<Amount>\d+)\)(?: -> (?:(?<Children>\w+)(?:, )?)+)?");
            var vertex = tree.DetermineRoot();
            WriteLn(vertex.Value);
        }

        public override void PartTwo()
        {
            var tree = Input.ToDigraph(s => s.Extract<(string Name, int Weight, List<string> Children)>(@"(\w+) \((\d+)\)(?: -> (?:(\w+)(?:, )?)+)?"),
                tuple => tuple.Name,
                tuple => );
            // var weightFunction = Data.Memoize<QuantityVertex<string>, long>(tree.TotalVertexWeight);
            // var incorrect = tree.Bfs().First(v => !v.Neighbors.Select(weightFunction).AllEqual());
            // var (min, max) = incorrect.Neighbors.Select(weightFunction).Frequencies().ExtentsBy(pair => pair.Value);
            // WriteLn(max.Key);
        }
    }
}