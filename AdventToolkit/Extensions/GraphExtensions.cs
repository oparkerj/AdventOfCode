using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class GraphExtensions
    {
        public static UniqueGraph<TT, Vertex<TT>, DirectedEdge<TT>> ToDigraph<T, TT>(this IEnumerable<T> source, Func<T, TT> parent, Func<T, TT> child)
        {
            var graph = new UniqueGraph<TT, Vertex<TT>, DirectedEdge<TT>>();
            foreach (var item in source)
            {
                _ = new DirectedEdge<TT>(graph.GetOrCreate(parent(item)), graph.GetOrCreate(child(item)));
            }
            return graph;
        }

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

        public static Vertex<T> GetOrCreate<T, TEdge>(this UniqueGraph<T, Vertex<T>, TEdge> graph, T value)
            where TEdge : Edge<T>
        {
            return graph.GetOrCreate(value, v => new Vertex<T>(v));
        }

        public static bool HasIncomingEdges<T, TVertex>(this Graph<T, TVertex, DirectedEdge<T>> graph, TVertex vertex)
            where TVertex : Vertex<T>
        {
            return vertex._edges.Any(edge => edge.To == vertex);
        }

        public static IEnumerable<TVertex> GetIncomingVertices<T, TVertex>(this Graph<T, TVertex, DirectedEdge<T>> graph, TVertex vertex)
            where TVertex : Vertex<T>
        {
            return vertex._edges.Where(edge => edge.Other(vertex) == null).Select(edge => (TVertex) edge.From);
        }
    }
}