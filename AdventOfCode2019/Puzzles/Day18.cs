using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq.Extensions;

namespace AdventOfCode2019.Puzzles
{
    public class Day18 : Puzzle
    {
        public const char You = '@';
        public const char Wall = '#';
        public const char Open = '.';

        public Grid<char> Map;

        public Day18()
        {
            Map = Input.ToGrid();
            Part = 2;
        }

        // Build graph from grid
        public UniqueDataGraph<char, int> MakeGraph()
        {
            var graph = new UniqueDataGraph<char, int>();
            foreach (var key in Map.Values.Where(c => char.IsLetter(c) || char.IsNumber(c)))
            {
                graph.AddVertex(new DataVertex<char, int>(key));
            }
            graph.AddVertex(new DataVertex<char, int>(You));
            foreach (var vertex in graph)
            {
                var keys = Map.Bfs(Map.Find(vertex.Value), pos => Map[pos] == Open, pos => char.IsLetter(Map[pos]) || char.IsNumber(Map[pos]) || Map[pos] == You, true);
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

        public int Solve<TPos>(UniqueDataGraph<char, int> graph, State<TPos> initial, Func<State<TPos>, IEnumerable<(TPos Pos, char Key, int Travel)>> possibilities)
        {
            // Perform a dijkstra search on (position, keys)
            var keyCount = graph.Values.Where(char.IsLower).Count();
            var best = new DefaultDict<(TPos, string), int> {DefaultValue = int.MaxValue};
            best[(initial.Pos, initial.Keys)] = 0;
            var queue = new SelfPriorityQueue<State<TPos>>(Comparing<State<TPos>>.By(state => state.Dist).ThenByReverse(state => state.Keys.Length));
            queue.Enqueue(initial);
            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                WriteLn(state.Keys);
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
                    var withKey = Enumerable.Append(state.Keys, key).Sorted().Str();
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

        public record State<TPos>(TPos Pos, int Dist, string Keys);

        public override void PartOne()
        {
            var graph = MakeGraph();
            var start = graph.Get(You);

            var result = Solve(graph, new State<DataVertex<char, int>>(start, 0, ""), state =>
            {
                var search = graph.ToDijkstra().ComputeWhere(state.Pos, vertex => char.IsLower(vertex.Value) || state.Keys.Contains(char.ToLower(vertex.Value)) || vertex.Value == You);
                var useless = search.Keys.Where(vertex => char.IsUpper(vertex.Value) || state.Keys.Contains(vertex.Value) || vertex.Value == You).ToList();
                foreach (var vertex in useless)
                {
                    search.Remove(vertex);
                }
                return search.Select(pair => (pair.Key, pair.Key.Value, pair.Value));
            });
            WriteLn(result);
        }

        public record Bots(DataVertex<char, int>[] Positions)
        {
            public Bots(IEnumerable<DataVertex<char, int>> items) : this(items.ToArray()) { }
            
            public override int GetHashCode()
            {
                return HashCode.Combine(Positions[0], Positions[1], Positions[2], Positions[3]);
            }

            public virtual bool Equals(Bots other)
            {
                if (other is null) return false;
                return Positions.EquiZip(other.Positions, (a, b) => a == b).AllEqual(true);
            }
        }

        public override void PartTwo()
        {
            // Fill in the walls
            var entrance = Map.Find(You);
            foreach (var pos in entrance.Adjacent())
            {
                Map[pos] = Wall;
            }
            var start = new[] {'1', '2', '3', '4'};
            foreach (var (pos, value) in entrance.Corners().Zip(start))
            {
                Map[pos] = value;
            }
            var graph = MakeGraph();

            IEnumerable<(Bots Pos, char Key, int Travel)> Possibilities(State<Bots> state)
            {
                var bots = state.Pos.Positions;
                for (var i = 0; i < bots.Length; i++)
                {
                    var search = graph.ToDijkstra().ComputeWhere(bots[i], vertex => char.IsLower(vertex.Value) || char.IsNumber(vertex.Value) || state.Keys.Contains(char.ToLower(vertex.Value)));
                    var useless = search.Keys.Where(vertex => char.IsUpper(vertex.Value) || char.IsNumber(vertex.Value) || state.Keys.Contains(vertex.Value)).ToList();
                    foreach (var vertex in useless)
                    {
                        search.Remove(vertex);
                    }
                    foreach (var (option, dist) in search)
                    {
                        var next = bots.ToArray();
                        next[i] = option;
                        yield return (new Bots(next), option.Value, dist);
                    }
                }
            }
            
            var result = Solve(graph, new State<Bots>(new Bots(start.Select(graph.Get)), 0, ""), Possibilities);
            WriteLn(result);
        }
    }
}