using AdventToolkit;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2023.Puzzles;

public class Day23 : Puzzle<int>
{
    public override int PartOne()
    {
        var grid = Input.ToGrid();
        var graph = new UniqueDataDigraph<Pos, int>();

        // Find the spaces where you can branch into multiple directions
        var junctions = grid.Where(pair => pair.Value == '.' && pair.Key.Adjacent().All(p => grid[p] != '.'))
            .Keys()
            .ToHashSet();

        // Add the start and end points
        var start = grid.View(grid.Bounds.XRange, grid.Bounds.MaxY).Find('.');
        var end = grid.View(grid.Bounds.XRange, grid.Bounds.MinY).Find('.');
        junctions.Add(start);
        junctions.Add(end);

        // Build a graph with collapsed edges
        var dijkstra = grid.SpaceToDijkstra();
        dijkstra.Neighbors = Part == 2 ? GetNeighborsIgnoreSlopes : GetNeighborsSloped;
        foreach (var junction in junctions)
        {
            var result = dijkstra.ComputeFindMany(junction, pos => junctions.Contains(pos) && pos != junction, (_, p) => grid.Has(p) && grid[p] != '#');
            foreach (var (other, dist) in result)
            {
                AddLink(junction, other, dist);
            }
        }
        
        return Dfs().Max();
        
        // Brute force (very slow)
        IEnumerable<int> Dfs()
        {
            var paths = new Stack<(Pos, int, HashSet<Pos>)>();
            paths.Push((start, 0, [start]));
            
            while (paths.Count > 0)
            {
                var (current, dist, visited) = paths.Pop();
                if (current == end)
                {
                    yield return dist;
                    continue;
                }
                foreach (var (next, nextDist) in GraphNeighbors(current, dist))
                {
                    if (!visited.Contains(next))
                    {
                        paths.Push((next, nextDist, [..visited, next]));
                    }
                }
            }
        }
        
        IEnumerable<(Pos, int)> GraphNeighbors(Pos pos, int offset)
        {
            var node = graph.Get(pos);
            return node.Neighbors.Select(other => (other.Value, node.GetEdge(other.Value).Data + offset));
        }
        
        IEnumerable<Pos> GetNeighborsSloped(Pos current)
        {
            foreach (var pos in current.Adjacent())
            {
                if (!grid.Has(pos)) continue;
                var tile = grid[pos];
                if (tile == '.')
                {
                    yield return pos;
                    continue;
                }
                if (tile == '#') continue;
                if (pos + Pos.RelativeDirection(tile) != current) yield return pos;
            }
        }
        
        IEnumerable<Pos> GetNeighborsIgnoreSlopes(Pos current)
        {
            return current.Adjacent().Where(pos => grid.Has(pos) && grid[pos] != '#');
        }

        void AddLink(Pos from, Pos to, int dist)
        {
            if (!graph.TryGet(from, out var fromVertex))
            {
                fromVertex = new SimpleVertex<Pos, DirectedDataEdge<Pos, int>>(from);
                graph.AddVertex(fromVertex);
            }
            if (!graph.TryGet(to, out var toVertex))
            {
                toVertex = new SimpleVertex<Pos, DirectedDataEdge<Pos, int>>(to);
                graph.AddVertex(toVertex);
            }
            fromVertex.LinkToDirected(toVertex, dist);
        }
    }
}