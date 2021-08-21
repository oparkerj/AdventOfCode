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

        public static Dijkstra<TVertex, WeightedEdge<T>> ToDijkstra<T, TVertex>(this Graph<T, TVertex, WeightedEdge<T>> graph)
            where TVertex : Vertex<T>
        {
            return new()
            {
                Neighbors = vertex => vertex._edges.Cast<WeightedEdge<T>>(),
                Distance = edge => edge.Weight,
                Cell = (vertex, edge) => edge.OtherAs(vertex)
            };
        }
    }
}