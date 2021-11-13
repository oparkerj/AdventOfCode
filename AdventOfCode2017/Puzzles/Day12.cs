using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;

namespace AdventOfCode2017.Puzzles
{
    public class Day12 : Puzzle
    {
        public Day12()
        {
            Part = 2;
        }

        public UniqueGraph<string> ReadGraph()
        {
            return Input.ToGraph(@"(?<Value>\d+) <-> (?:(?<Children>\d+)(?:, )?)+");
        }

        public override void PartOne()
        {
            var graph = ReadGraph();
            var result = graph.Reachable(graph.Get("0")).Count();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var groups = 0;
            var seen = new HashSet<string>();
            var graph = ReadGraph();
            foreach (var vertex in graph)
            {
                if (seen.Contains(vertex.Value)) continue;
                groups++;
                foreach (var v in graph.Reachable(vertex))
                {
                    seen.Add(v.Value);
                }
            }
            WriteLn(groups);
        }
    }
}