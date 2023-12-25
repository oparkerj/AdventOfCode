using AdventToolkit;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day25 : Puzzle<int>
{
    public override int PartOne()
    {
        var graph = new UniqueDigraph<string>();

        foreach (var s in Input)
        {
            var name = s.Before(':');
            var node = graph.GetOrCreate(name);
            foreach (var other in s.After(':').Spaced())
            {
                var otherNode = graph.GetOrCreate(other);
                node.LinkTo(otherNode, new DirectedEdge<string>(node, otherNode));
            }
        }

        // Found which edges to remove by manual inspection
        RemoveEdge("ptq", "fxn");
        RemoveEdge("szl", "kcn");
        RemoveEdge("fbd", "lzd");
        var one = graph.ReachableIgnoreDirection(graph.Get("ptq")).Count();
        var two = graph.ReachableIgnoreDirection(graph.Get("fbd")).Count();
        return one * two;

        void RemoveEdge(string a, string b)
        {
            var aNode = graph.Get(a);
            var bNode = graph.Get(b);
            var edge = aNode.GetEdge(b);
            aNode.RemoveEdge(edge);
            bNode.RemoveEdge(edge);
        }
    }
}