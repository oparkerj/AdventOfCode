using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Tree;
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
            var tree = Input.ToQuantityTree(@"(?<Value>\w+) \((?<Amount>\d+)\)(?: -> (?:(?<Children>\w+)(?:, )?)+)?");
            var vertex = tree.DetermineRoot();
            WriteLn(vertex.Value);
        }

        private long FindCorrectValue(QuantityVertex<string> vertex)
        {
            var count = vertex.NeighborCount;
            if (count == 2) return vertex.Neighbors.Select(FindCorrectValue).Max();
            if (count < 2) return -1;
            // Return branches grouped by weight
            var counts = vertex.NeighborEdges.With(edge => edge.Data + edge.OtherAs(vertex).SumBranches())
                .GroupBy(pair => pair.Value)
                .Select(pairs => pairs.Keys().ToArray())
                .ToArray();
            if (counts.Length == 1 || counts.AllEqual(b => b.Length)) return -1;
            // The single branch that is different from the others
            var offBranch = counts.Single(b => b.Length == 1)[0].OtherAs(vertex);
            // Find whether the wrong value is further up the chain
            var chosen = FindCorrectValue(offBranch);
            if (chosen != -1) return chosen;
            // Compute the different between the wrong value and the correct values
            var regularEdge = counts.First(b => b.Length != 1)[0];
            return regularEdge.Data + regularEdge.OtherAs(vertex).SumBranches() - (offBranch.Quantity + offBranch.SumBranches()) + offBranch.Quantity;
        }

        public override void PartTwo()
        {
            var weights = new Dictionary<string, long>();
            var programs = Input.Extract<(string, long)>(@"(\w+) \((\d+)\).*");
            foreach (var (value, amount) in programs)
            {
                weights[value] = amount;
            }
            var tree = Input.Extract<(string, List<string>)>(@"(\w+) \(\d+\)(?: -> (?:(\w+)(?:, )?)+)?")
                .ToQuantityTree<(string Value, List<string> Children), string>((tuple, helper) =>
                {
                    helper.Add(tuple.Value);
                    foreach (var child in tuple.Children)
                    {
                        helper.AddChild(child, weights[child]);
                    }
                });
            
            var root = tree.DetermineRoot();
            var result = FindCorrectValue(root);
            WriteLn(result);
        }
    }
}