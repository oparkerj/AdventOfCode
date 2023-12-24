using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2016.Puzzles;

public class Day24 : Puzzle
{
    public const char Wall = '#';
    public const char Open = '.';
    
    public Grid<char> Map;

    public Day24()
    {
        Map = Input.ToGrid();
    }
    
    // Modified from 2019 Day 18
    public UniqueDataGraph<char, int> MakeGraph()
    {
        var graph = new UniqueDataGraph<char, int>();
        foreach (var key in Map.Values.Where(char.IsNumber))
        {
            graph.AddVertex(new DataVertex<char, int>(key));
        }
        foreach (var vertex in graph)
        {
            var keys = Map.Bfs(Map.Find(vertex.Value), pos => Map[pos] == Open, pos => char.IsNumber(Map[pos]), true);
            foreach (var key in keys)
            {
                var other = graph.Get(Map[key]);
                if (vertex.ConnectedTo(other)) continue;
                var dist = Map.ShortestPathBfs(Map.Find(vertex.Value), key, pos => Map[pos] != Wall);
                vertex.LinkTo(other, new DataEdge<char, int>(vertex, other, dist));
            }
        }
        return graph;
    }
    
    // Modified from 2019 Day 18
    public int Solve<TPos>(UniqueDataGraph<char, int> graph, State<TPos> initial, Func<State<TPos>, IEnumerable<(TPos Pos, char Key, int Travel)>> possibilities)
    {
        // Perform a dijkstra search on (position, keys)
        var keyCount = graph.Values.Where(char.IsNumber).Count();
        var best = new DefaultDict<(TPos, string), int> {DefaultValue = int.MaxValue};
        best[(initial.Pos, initial.Keys)] = 0;
        var queue = new SelfPriorityQueue<State<TPos>>(Comparing<State<TPos>>.By(state => state.Dist).ThenByReverse(state => state.Keys.Length));
        queue.Enqueue(initial);
        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (state.Keys.Length == keyCount)
            {
                return state.Dist;
            }
            var lookup = (state.Pos, state.Keys);
            // If this state is in the same position with a worse distance, skip
            if (state.Dist > best[lookup]) continue;
            // Add possibility of collecting each key from this state if
            // we have found a better distance to collect the key from this state.
            foreach (var (pos, key, travel) in possibilities(state))
            {
                // For part 2, only allow 0 to be collected last
                if (Part == 2 && key == '0' && state.Keys.Length < keyCount - 1) continue;
                var withKey = state.Keys.Append(key).Sorted().Str();
                var withTravel = state.Dist + travel;
                lookup = (pos, withKey);
                if (withTravel < best[lookup])
                {
                    best[lookup] = withTravel;
                    queue.Enqueue(new State<TPos>(pos, withTravel, withKey));
                }
            }
        }
        return -1;
    }
    
    // Copied from 2019 Day 18
    public record State<TPos>(TPos Pos, int Dist, string Keys);

    // Modified from 2019 Day 18
    public override void PartOne()
    {
        var graph = MakeGraph();
        var start = graph.Get('0');

        var result = Solve(graph, new State<DataVertex<char, int>>(start, 0, "0"), state =>
        {
            var search = graph.ToDijkstraDataEdge().ComputeWhere(state.Pos, vertex => char.IsNumber(vertex.Value) || state.Keys.Contains(vertex.Value));
            var useless = search.Keys.Where(vertex => state.Keys.Contains(vertex.Value)).ToList();
            foreach (var vertex in useless)
            {
                search.Remove(vertex);
            }
            return search.Select(pair => (pair.Key, pair.Key.Value, pair.Value));
        });
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var graph = MakeGraph();
        var start = graph.Get('0');

        var result = Solve(graph, new State<DataVertex<char, int>>(start, 0, ""), state =>
        {
            var search = graph.ToDijkstraDataEdge().ComputeWhere(state.Pos, vertex => char.IsNumber(vertex.Value) || state.Keys.Contains(vertex.Value));
            var useless = search.Keys.Where(vertex => state.Keys.Contains(vertex.Value)).ToList();
            foreach (var vertex in useless)
            {
                search.Remove(vertex);
            }
            return search.Select(pair => (pair.Key, pair.Key.Value, pair.Value));
        });
        WriteLn(result);
    }
}