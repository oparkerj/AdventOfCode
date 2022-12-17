using AdventToolkit;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day16 : Puzzle<int>
{
    public UniqueDataGraph<string, int> Relevant;
    public Dictionary<string, int> Flow;
    public Func<int, string, string, int> Cache;

    public void ReadInput()
    {
        var graph = new UniqueGraph<string>();
        Flow = new Dictionary<string, int>();
        foreach (var s in Input)
        {
            var valve = s.After("Valve ").Extract<string>(@"\w+");
            var flow = s.After("rate=").TakeInt();
            var others = s.After("valve").Substring(1).TrimStart().Csv(true);

            Flow[valve] = flow;

            var vertex = graph.GetOrCreate(valve);
            foreach (var other in others)
            {
                var otherVertex = graph.GetOrCreate(other);
                if (!vertex.ConnectedTo(otherVertex))
                {
                    vertex.LinkTo(otherVertex);
                }
            }
        }

        Relevant = new UniqueDataGraph<string, int>();
        var nodes = Flow.WhereValue(i => i != 0).Keys().Append("AA").ToList();
        var dijkstra = graph.ToDijkstra();
        foreach (var node in nodes)
        {
            var r = Relevant.GetOrCreate(node, s => new DataVertex<string, int>(s));
            foreach (var other in nodes)
            {
                var o = Relevant.GetOrCreate(other, s => new DataVertex<string, int>(s));
                if (r.ConnectedTo(o)) continue;
                var dist = node == other ? 0 : dijkstra.ComputeFind(graph[node], graph[other]);
                r.LinkTo(o, (a, b, d) => new DataEdge<string, int>(a, b, d), dist);
            }
        }
        
        Cache = Data.Memoize<int, string, string, int>(Search);
    }
    
    int Search(int timeRemaining, string current, string closed)
    {
        var v = Relevant[current];
        var best = closed.Csv()
            .Select(s => v.GetEdge(s))
            .Where(edge => edge.Data < timeRemaining)
            .Select(edge => (edge.Other(v).Value, TimeLeft: timeRemaining - edge.Data - 1))
            .Select(next => Flow[next.Value] * next.TimeLeft + Cache(next.TimeLeft, next.Value, closed.Cut(next.Value, 1).Trim(',')))
            .MaxOrDefault();
        return best;
    }

    public override int PartOne()
    {
        ReadInput();
        return Cache(30, "AA", string.Join(',', Relevant.Values.Without("AA")));
    }

    public override int PartTwo()
    {
        ReadInput();

        // int Search(int timeRemaining, string current, string closed, bool includeElephant)
        // {
        //     var v = Relevant[current];
        //     var best = closed.Csv()
        //         .Select(s => v.GetEdge(s))
        //         .Where(edge => edge.Data < timeRemaining)
        //         .Select(edge => (edge.Other(v).Value, TimeLeft: timeRemaining - edge.Data - 1))
        //         .Select(next => Flow[next.Value] * next.TimeLeft + cache.Data(next.TimeLeft, next.Value, closed.Cut(next.Value, 1).Trim(','), includeElephant))
        //         .Append(cache.Data(26, "AA", closed, false))
        //         .MaxOrDefault();
        //     return best;
        // }

        // This is the above code converted to not use recursion, otherwise stack overflow.
        var cache = new Dictionary<(int, string, string, bool), int>();
        var stack = new Stack<(List<int>, IEnumerator<(string Value, int TimeLeft)>, int, string, string, bool)>();
        stack.Push((new List<int>(), null, 26, "AA", string.Join(',', Relevant.Values.Without("AA")), true));
        var result = -1;
        while (stack.Count > 0)
        {
            var (options, next, timeRemaining, current, closed, includeElephant) = stack.Peek();
            if (cache.TryGetValue((timeRemaining, current, closed, includeElephant), out var retval))
            {
                next?.Dispose();
                result = retval;
                stack.Pop();
                continue;
            }
            var hasResult = next != null;
            if (!hasResult)
            {
                var v = Relevant[current];
                next = closed.Csv()
                    .Select(s => v.GetEdge(s))
                    .Where(edge => edge.Data < timeRemaining)
                    .Select(edge => (edge.Other(v).Value, TimeLeft: timeRemaining - edge.Data - 1))
                    .GetEnumerator();
                stack.Replace((options, next, timeRemaining, current, closed, includeElephant));
            }

            if (hasResult) options.Add(Flow[next.Current.Value] * next.Current.TimeLeft + result);
            if (next.MoveNext())
            {
                stack.Push((new List<int>(), null, next.Current.TimeLeft, next.Current.Value, closed.Cut(next.Current.Value, 1).Trim(','), includeElephant));
            }
            else
            {
                next.Dispose();
                options.Add(Cache(26, "AA", closed));
                result = options.Max();
                cache[(timeRemaining, current, closed, includeElephant)] = result;
                stack.Pop();
            }
        }

        return result;
    }
}