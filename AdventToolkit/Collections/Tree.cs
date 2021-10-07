using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventToolkit.Collections
{
    public class Tree<T, TVertex, TEdge> : UniqueGraph<T, TVertex, TEdge>
        where TVertex : TreeVertex<T, TEdge>
        where TEdge : Edge<T>
    {
        public TVertex Root { get; protected set; }

        public override void AddVertex(TVertex vertex)
        {
            base.AddVertex(vertex);
            Root ??= vertex;
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

    public class TreeVertex<T, TEdge> : SimpleVertex<T, TEdge>
        where TEdge : Edge<T>
    {
        public TreeVertex<T, TEdge> Parent { get; set; }
        public TEdge ParentEdge { get; private set; }

        public TreeVertex() { }

        public TreeVertex(T value) : base(value) { }

        public int Height
        {
            get
            {
                var height = 0;
                var parent = Parent;
                while (parent != null)
                {
                    height++;
                    parent = parent.Parent;
                }
                return height;
            }
        }

        public IEnumerable<TreeVertex<T, TEdge>> Parents
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    yield return parent;
                    parent = parent.Parent;
                }
            }
        }

        public new IEnumerable<TreeVertex<T, TEdge>> Neighbors => base.Neighbors.Cast<TreeVertex<T, TEdge>>();

        public new IEnumerable<TreeVertex<T, TEdge>> Connected => base.Connected.Cast<TreeVertex<T, TEdge>>();

        public override IEnumerable<TEdge> Edges => ParentEdge == null ? base.Edges : base.Edges.Prepend(ParentEdge);

        public virtual TreeVertex<T, TEdge> GetChild(TEdge link) => link.OtherAs(this);

        public override void LinkTo(Vertex<T, TEdge> other, TEdge edge)
        {
            AddEdge(edge);
        }

        public override void AddEdge(TEdge edge)
        {
            base.AddEdge(edge);
            var other = edge.OtherAs(this);
            other.Parent = this;
            other.ParentEdge = edge;
        }

        public override void RemoveEdge(TEdge edge)
        {
            if (edge == ParentEdge)
            {
                Parent.RemoveEdge(ParentEdge);
                Parent = null;
                ParentEdge = null;
            }
            else base.RemoveEdge(edge);
        }

        public override bool Unlink(Vertex<T, TEdge> other)
        {
            if (other != null && other == Parent)
            {
                RemoveEdge(ParentEdge);
                return true;
            }
            return base.Unlink(other);
        }

        public bool HasChild(T value)
        {
            return Descendants.Any(vertex => Equals(vertex.Value, value));
        }

        public IEnumerable<TreeVertex<T, TEdge>> Descendants => Neighbors.Concat(Neighbors.SelectMany(vertex => vertex.Descendants));

        public IEnumerable<TEdge> DescendantLinks => Edges.Concat(Edges.Select(GetChild).SelectMany(vertex => vertex.DescendantLinks));
    }
}