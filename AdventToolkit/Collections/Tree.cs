using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;

namespace AdventToolkit.Collections
{
    public class Tree<T, TVertex, TLink, TEdge> : Graph<T, TVertex, TEdge>
        where TVertex : TreeVertex<T, TLink>
        where TEdge : Edge<T>
    {
        
    }

    public abstract class TreeVertex<T, TLink> : SimpleVertex<T>
    {
        public TreeVertex<T> Parent { get; set; }
        public Edge<T> ParentEdge { get; private set; }
        
        public readonly List<Edge<T>> Links = new(2);

        protected TreeVertex(T value) : base(value) { }

        public override int Count => Links.Count;

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

        public IEnumerable<TreeVertex<T>> Parents
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

        public override IEnumerable<Vertex<T>> Neighbors => Children;

        public override IEnumerable<Edge<T>> Edges => Links.Prepend(ParentEdge);

        public override void AddEdge(Edge<T> edge)
        {
            Links.Add(edge);
        }

        public abstract TreeVertex<T> GetChild(Edge<T> link);

        public virtual void AddChild(Edge<T> child) => Links.Add(child);

        public bool HasChild(T value)
        {
            return Descendants.Any(vertex => Equals(vertex.Value, value));
        }

        public IEnumerable<TreeVertex<T>> Children => Links.Select(GetChild);
        
        public IEnumerable<TreeVertex<T>> Descendants => Children.Concat(Children.SelectMany(vertex => vertex.Descendants));

        public IEnumerable<Edge<T>> DescendantLinks => Links.Concat(Links.Select(GetChild).SelectMany(vertex => vertex.DescendantLinks));
    }
}