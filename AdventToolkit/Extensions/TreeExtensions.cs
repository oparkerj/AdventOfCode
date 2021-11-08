using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Collections.Tree;
using RegExtract;

namespace AdventToolkit.Extensions
{
    public static class TreeExtensions
    {
        public static Tree<TT> ToTree<T, TT>(this IEnumerable<T> items, Func<T, TT> parent, Func<T, TT> child)
        {
            var tree = new Tree<TT>();
            foreach (var item in items)
            {
                tree.GetVertex(parent(item)).LinkTo(tree.GetVertex(child(item)));
            }
            return tree;
        }

        public static Tree<TT> ToTree<T, TI, TT>(this IEnumerable<T> items, Func<T, TI> func, Func<TI, TT> parent, Func<TI, TT> child)
        {
            var tree = new Tree<TT>();
            foreach (var item in items)
            {
                var i = func(item);
                tree.GetVertex(parent(i)).LinkTo(tree.GetVertex(child(i)));
            }
            return tree;
        }

        public static Tree<string> ToTree(this IEnumerable<string> items, string format)
        {
            return items.Extract<VertexInfo<string>>(format)
                .ToTree(info => info.Value, info => info.Child);
        }

        public static IEnumerable<T> Values<T>(this IEnumerable<VertexBase<T>> nodes)
        {
            return nodes.Select(node => node.Value);
        }

        public static TVertex CommonAncestor<T, TVertex, TEdge>(this Tree<T, TVertex, TEdge> tree, T a, T b)
            where TVertex : TreeVertex<T, TEdge>
            where TEdge : Edge<T>
        {
            if (!tree.TryGet(a, out var av) || !tree.TryGet(b, out var bv)) return null;
            var ap = av.Parent;
            var bp = bv.Parent;
            if (ap == bp) return ap as TVertex;
            var seen = new HashSet<TreeVertex<T, TEdge>> {ap, bp};
            while (ap != null || bp != null)
            {
                ap = ap?.Parent;
                bp = bp?.Parent;
                if (ap != null)
                {
                    if (seen.Contains(ap)) return ap as TVertex;
                    seen.Add(ap);
                }
                if (bp != null)
                {
                    if (seen.Contains(bp)) return bp as TVertex;
                    seen.Add(bp);
                }
            }
            return null;
        }
    }
}