using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day9 : Puzzle
{
    public Dictionary<(string, string), int> Graph = new();

    public Day9()
    {
        BuildGraph();
    }

    public void BuildGraph()
    {
        Graph = new Dictionary<(string, string), int>();
        foreach (var (a, b, dist) in Input.Extract<(string, string, int)>(@"(\w+) to (\w+) = (\d+)"))
        {
            Graph[(a, b).Sorted()] = dist;
        }
    }

    public IEnumerable<int> Paths()
    {
        var places = Graph.Keys.UnpackAll().Distinct();
        return places.Permutations().Select(list => list.Pairwise((a, b) => Graph[(a, b).Sorted()]).Sum());
    }

    public override void PartOne() => WriteLn(Paths().Min());

    public override void PartTwo() => WriteLn(Paths().Max());
}