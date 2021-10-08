using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class GraphExtensions
    {
        public static UniqueGraph<TT, Vertex<TT, DirectedEdge<TT>>, DirectedEdge<TT>> ToDigraph<T, TT>(this IEnumerable<T> source, Func<T, TT> parent, Func<T, TT> child)
        {
            var graph = new UniqueGraph<TT, Vertex<TT, DirectedEdge<TT>>, DirectedEdge<TT>>();
            foreach (var item in source)
            {
                graph.GetOrCreate(parent(item)).LinkTo(graph.GetOrCreate(child(item)));
            }
            return graph;
        }

        public static IEnumerable<TVertex> Reachable<T, TVertex, TEdge>(this TVertex start, Func<TVertex, bool> valid)
            where TVertex : Vertex<T, TEdge>
            where TEdge : Edge<T>
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
                    .Select(edge => edge.OtherAs(current))
                    .Where(valid)
                    .Where(v => !visited.Contains(v));
                foreach (var next in neighbors)
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        public static Dijkstra<TVertex, DataEdge<T, int>> ToDijkstra<T, TVertex>(this Graph<T, TVertex, DataEdge<T, int>> graph)
            where TVertex : Vertex<T, DataEdge<T, int>>
        {
            return new()
            {
                Neighbors = vertex => vertex.Edges,
                Distance = edge => edge.Data,
                Cell = (vertex, edge) => edge.OtherAs(vertex)
            };
        }
        
        public static Dijkstra<TVertex, Edge<T>> ToDijkstra<T, TVertex>(this Graph<T, TVertex, Edge<T>> graph)
            where TVertex : Vertex<T, Edge<T>>
        {
            return new()
            {
                Neighbors = vertex => vertex.Edges,
                Distance = _ => 1,
                Cell = (vertex, edge) => edge.OtherAs(vertex)
            };
        }

        public static Vertex<T, TEdge> GetOrCreate<T, TEdge>(this UniqueGraph<T, Vertex<T, TEdge>, TEdge> graph, T value)
        
            where TEdge : Edge<T>
        {
            return graph.GetOrCreate(value, v => new SimpleVertex<T, TEdge>(v));
        }

        // Unused first parameter restricts calls to directed graphs.
        public static bool HasIncomingEdges<T, TVertex>(this Graph<T, TVertex, DirectedEdge<T>> graph, TVertex vertex)
            where TVertex : Vertex<T, DirectedEdge<T>>
        {
            return vertex.Edges.Any(edge => edge.To == vertex);
        }

        public static IEnumerable<TVertex> GetIncomingVertices<T, TVertex>(this Graph<T, TVertex, DirectedEdge<T>> graph, TVertex vertex)
            where TVertex : Vertex<T, DirectedEdge<T>>
        {
            return vertex.Edges.Where(edge => edge.Other(vertex) == null).Select(edge => (TVertex) edge.From);
        }

        public static SimpleVertex<T, TEdge> AddVertex<T, TEdge>(this Graph<T, SimpleVertex<T, TEdge>, TEdge> graph, T value)
            where TEdge : Edge<T>
        {
            var v = new SimpleVertex<T, TEdge>(value);
            graph.AddVertex(v);
            return v;
        }

        public static void LinkTo<T>(this Vertex<T, Edge<T>> vertex, Vertex<T, Edge<T>> other)
        {
            vertex.LinkTo(other, (a, b) => new Edge<T>(a, b));
        }
        
        public static void LinkTo<T>(this Vertex<T, DirectedEdge<T>> vertex, Vertex<T, DirectedEdge<T>> other)
        {
            vertex.LinkTo(other, (a, b) => new DirectedEdge<T>(a, b));
        }

        public static void LinkTo<T, TEdge>(this Vertex<T, TEdge> vertex, Vertex<T, TEdge> other, Func<Vertex<T, TEdge>, Vertex<T, TEdge>, TEdge> cons)
            where TEdge : Edge<T>
        {
            var edge = cons(vertex, other);
            vertex.LinkTo(other, edge);
        }

        public static void LinkTo<T, TEdge, TData>(this Vertex<T, TEdge> vertex, Vertex<T, TEdge> other, Func<Vertex<T, TEdge>, Vertex<T, TEdge>, TData, TEdge> cons, TData data)
            where TEdge : Edge<T>, IEdgeData<TData>
        {
            vertex.LinkTo(other, (a, b) => cons(a, b, data));
        }
    }
}