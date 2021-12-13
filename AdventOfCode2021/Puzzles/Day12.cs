using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Utilities;

namespace AdventOfCode2021.Puzzles;

public class Day12 : Puzzle
{
    public UniqueGraph<string> Graph;

    public Day12()
    {
        Part = 2;
        Graph = Input.Select(s => s.SingleSplit('-')).ToGraph(tuple => tuple.Left, tuple => tuple.Right);
    }

    public override void PartOne()
    {
        var dijkstra = new Dijkstra<State, string>
        {
            Neighbors = state => Graph[state.Current].NeighborValues.Where(s => !state.Seen.Contains(s)).Without("start"),
            Distance = _ => 1,
            Cell = (state, s) => new State(s, char.IsLower(s[0]) ? state.Seen + "," + s : state.Seen, state.Path + "," + s)
        };
        var dictionary = dijkstra.Compute(new State("start", "", ""));
        WriteLn(dictionary.Keys.Count(state => state.Current == "end"));
    }
    
    public record State(string Current, string Seen, string Path);

    public int CountPathsFrom(string current, List<string> seen, string twice)
    {
        if (current == "end") return 1;
        if (char.IsLower(current[0]) && seen.Contains(current))
        {
            if (twice == null) twice = current;
            else return 0;
        }
        seen = seen.Append(current).ToList();
        return Graph[current].NeighborValues.Without("start")
            .Sum(neighbor => CountPathsFrom(neighbor, seen, twice));
    }

    public override void PartTwo()
    {
        WriteLn(CountPathsFrom("start", new List<string>(), null));
    }
}