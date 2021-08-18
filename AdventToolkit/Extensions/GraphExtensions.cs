using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class GraphExtensions
    {
        public static IEnumerable<TVertex> Reachable<T, TVertex>(this TVertex start, Func<TVertex, bool> valid)
            where TVertex : Vertex<T>
        {
            var visited = new HashSet<TVertex>();
            var queue = new Queue<TVertex>();
            queue.Enqueue(start);
            visited.Add(start);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                var neighbors = current.Edges
                    .Select(edge => (TVertex) edge.Other(current))
                    .Where(valid)
                    .Where(v => !visited.Contains(v));
                foreach (var next in neighbors)
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        // Assumes the graph is using WeightedEdge
        // will throw an exception otherwise.
        public static Dictionary<TVertex, int> Dijkstra<T, TVertex>(this Graph<T, TVertex> graph, TVertex start)
            where TVertex : Vertex<T>
        {
            var dist = new Dictionary<TVertex, int> {[start] = 0};
            foreach (var vertex in graph)
            {
                dist[vertex] = int.MaxValue;
            }
            var queue = new PriorityQueue<TVertex>(dist.Select(pair => (pair.Key, pair.Value)));
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                var current = dist[v];
                foreach (var edge in v.Edges)
                {
                    var u = edge.OtherAs(v);
                    var weight = ((WeightedEdge<T>) edge).Weight;
                    if (current + weight < dist[u])
                    {
                        dist[u] = current + weight;
                    }
                }
            }
            return dist;
        }
        
        // Dijkstra but only explores valid neighbors
        public static Dictionary<TVertex, int> DijkstraWhere<T, TVertex>(this Graph<T, TVertex> graph, TVertex start, Func<TVertex, bool> valid = null)
            where TVertex : Vertex<T>
        {
            valid ??= _ => true;
            var dist = new DefaultDict<TVertex, int> {DefaultValue = int.MaxValue, [start] = 0};
            var seen = new HashSet<TVertex>();
            var queue = new PriorityQueue<TVertex>();
            queue.Enqueue(start, 0);
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                seen.Add(v);
                var current = dist[v];
                foreach (var edge in v.Edges)
                {
                    var u = edge.OtherAs(v);
                    if (!valid(u)) continue;
                    var weight = ((WeightedEdge<T>) edge).Weight;
                    if (!seen.Contains(u)) queue.Enqueue(u, current + weight);
                    if (current + weight < dist[u])
                    {
                        dist[u] = current + weight;
                    }
                }
            }
            return dist;
        }
        
        // Dijkstra but includes the vertex used to get the smallest distance to another vertex.
        public static Dictionary<TVertex, (int Dist, TVertex From)> DijkstraFrom<T, TVertex>(this Graph<T, TVertex> graph, TVertex start)
            where TVertex : Vertex<T>
        {
            var dist = new Dictionary<TVertex, (int Dist, TVertex From)> {[start] = (0, null)};
            foreach (var vertex in graph)
            {
                dist[vertex] = (int.MaxValue, null);
            }
            var queue = new PriorityQueue<TVertex>(dist.Select(pair => (pair.Key, pair.Value.Dist)));
            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                var current = dist[v].Dist;
                foreach (var edge in v.Edges)
                {
                    var u = edge.OtherAs(v);
                    var weight = ((WeightedEdge<T>) edge).Weight;
                    if (current + weight < dist[u].Dist)
                    {
                        dist[u] = (current + weight, v);
                    }
                }
            }
            return dist;
        }
    }
}