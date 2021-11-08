using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Tree
{
    public class Tree<T, TVertex, TEdge> : UniqueGraph<T, TVertex, TEdge>
        where TVertex : TreeVertex<T, TEdge>
        where TEdge : Edge<T>
    {
        public TVertex Root { get; protected set; }

        public TVertex DetermineRoot()
        {
            if (!this.First(out var v)) return null;
            while (v.Parent is TVertex parent)
            {
                v = parent;
            }
            return Root = v;
        }

        public override void AddVertex(TVertex vertex)
        {
            base.AddVertex(vertex);
            Root ??= vertex;
        }

        public IEnumerable<TVertex> Bfs(TVertex start = null)
        {
            var next = new Queue<TVertex>();
            next.Enqueue(start ?? Root);
            while (next.Count > 0)
            {
                var v = next.Dequeue();
                yield return v;
                foreach (var n in v.Neighbors.Cast<TVertex>())
                {
                    next.Enqueue(n);
                }
            }
        }
    }

    public class Tree<T> : Tree<T, TreeVertex<T, Edge<T>>, Edge<T>>
    {
        public TreeVertex<T, Edge<T>> GetVertex(T item)
        {
            if (TryGet(item, out var vertex)) return vertex;
            var v = new TreeVertex<T, Edge<T>>(item);
            AddVertex(v);
            return v;
        }

        public TreeVertex<T, Edge<T>> NewNode()
        {
            var v = new TreeVertex<T, Edge<T>>();
            AddVertex(v);
            return v;
        }
    }
}