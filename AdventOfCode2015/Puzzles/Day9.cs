using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day9 : Puzzle<int>
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

    public int Dist(string a, string b) => Graph[(a, b).Sorted()];

    public IEnumerable<int> Paths()
    {
        var places = Graph.Keys.UnpackAll().Distinct();
        return places.Permutations().Select(list => list.Pairwise(Dist).Sum());
    }

    public override int PartOne() => Paths().Min();

    public override int PartTwo() => Paths().Max();
}