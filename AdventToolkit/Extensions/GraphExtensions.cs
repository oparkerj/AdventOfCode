using System;
using System.Collections.Generic;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class GraphExtensions
    {
        public static UniqueGraph<TT, VertexOld<TT>, DirectedEdgeOld<TT>> ToDigraph<T, TT>(this IEnumerable<T> source, Func<T, TT> parent, Func<T, TT> child)
        {
            var graph = new UniqueGraph<TT, VertexOld<TT>, DirectedEdgeOld<TT>>();
            foreach (var item in source)
            {
                _ = new DirectedEdgeOld<TT>(graph.GetOrCreate(parent(item)), graph.GetOrCreate(child(item)));
            }
            return graph;
        }

        public static IEnumerable<TVertex> Reachable<T, TVertex>(this TVertex start, Func<TVertex, bool> valid)
            where TVertex : VertexOld<T>
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

        public static Dijkstra<TVertex, WeightedEdgeOld<T>> ToDijkstra<T, TVertex>(this Graph<T, TVertex, WeightedEdgeOld<T>> graph)
            where TVertex : VertexOld<T>
        {
            return new()
            {
                Neighbors = vertex => vertex._edges.Cast<WeightedEdgeOld<T>>(),
                Distance = edge => edge.Weight,
                Cell = (vertex, edge) => edge.OtherAs(vertex)
            };
        }
        
        public static Dijkstra<TVertex, EdgeOld<T>> ToDijkstra<T, TVertex>(this Graph<T, TVertex, EdgeOld<T>> graph)
            where TVertex : VertexOld<T>
        {
            return new()
            {
                Neighbors = vertex => vertex._edges,
                Distance = _ => 1,
                Cell = (vertex, edge) => edge.OtherAs(vertex)
            };
        }

        public static VertexOld<T> GetOrCreate<T, TEdge>(this UniqueGraph<T, VertexOld<T>, TEdge> graph, T value)
            where TEdge : EdgeOld<T>
        {
            return graph.GetOrCreate(value, v => new VertexOld<T>(v));
        }

        // Unused first parameter restricts calls to directed graphs.
        public static bool HasIncomingEdges<T, TVertex>(this Graph<T, TVertex, DirectedEdgeOld<T>> graph, TVertex vertex)
            where TVertex : VertexOld<T>
        {
            return vertex._edges.Any(edge => edge.To == vertex);
        }

        public static IEnumerable<TVertex> GetIncomingVertices<T, TVertex>(this Graph<T, TVertex, DirectedEdgeOld<T>> graph, TVertex vertex)
            where TVertex : VertexOld<T>
        {
            return vertex._edges.Where(edge => edge.Other(vertex) == null).Select(edge => (TVertex) edge.From);
        }

        public static VertexOld<T> AddVertex<T, TEdge>(this Graph<T, VertexOld<T>, TEdge> graph, T value)
            where TEdge : EdgeOld<T>
        {
            var v = new VertexOld<T>(value);
            graph.AddVertex(v);
            return v;
        }
    }
}