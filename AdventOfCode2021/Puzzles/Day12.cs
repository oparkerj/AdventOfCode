using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Utilities;

namespace AdventOfCode2021.Puzzles;

public class Day12 : Puzzle
{
    public Day12()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var graph = Input.Select(s => s.SingleSplit('-')).ToGraph(tuple => tuple.Left, tuple => tuple.Right);
        var dijkstra = new Dijkstra<State, string>
        {
            Neighbors = state => graph[state.Current].NeighborValues.Where(s => !state.Seen.Contains(s)).Without("start"),
            Distance = _ => 1,
            Cell = (state, s) => new State(s, char.IsLower(s[0]) ? state.Seen + "," + s : state.Seen, state.Path + "," + s)
        };
        var dictionary = dijkstra.Compute(new State("start", "", ""));
        Clip(dictionary.Keys.Count(state => state.Current == "end"));
    }
    
    public record State(string Current, string Seen, string Path);

    public int CountPathsFrom(UniqueGraph<string> graph, string current, List<string> seen, string twice)
    {
        if (current == "end") return 1;
        if (char.IsLower(current[0]) && seen.Contains(current))
        {
            if (twice == null) twice = current;
            else return 0;
        }
        seen = seen.Append(current).ToList();
        return graph[current].NeighborValues.Without("start").Sum(neighbor => CountPathsFrom(graph, neighbor, seen, twice));
    }

    public override void PartTwo()
    {
        var graph = Input.Select(s => s.SingleSplit('-')).ToGraph(tuple => tuple.Left, tuple => tuple.Right);
        Clip(CountPathsFrom(graph, "start", new List<string>(), null));
    }
}