using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public abstract class Tree<T, TNode, TLink> : IEnumerable<TNode>
        where TNode : Node<T, TLink>
    {
        protected readonly Dictionary<T, TNode> Nodes = new();

        public void Add(TNode node)
        {
            Nodes[node.Value] = node;
        }

        public bool TryGet(T item, out TNode node)
        {
            return Nodes.TryGetValue(item, out node);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TNode> GetEnumerator() => Nodes.Values.GetEnumerator();
    }

    public abstract class Node<T, TLink> : IEnumerable<TLink>
    {
        public Node<T, TLink> Parent;
        protected readonly List<TLink> Links = new(2);

        public readonly T Value;

        public Node(T value) => Value = value;

        public abstract Node<T, TLink> LinkChild(TLink link);

        public virtual void AddChild(TLink child)
        {
            Links.Add(child);
        }

        public bool HasChild(T value)
        {
            return AllChildren.Any(node => Equals(node.Value, value));
        }

        public int Count => Links.Count;

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
        
        public IEnumerable<Node<T, TLink>> Parents
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

        public IEnumerable<Node<T, TLink>> Children => Links.Select(LinkChild);

        public IEnumerable<Node<T, TLink>> AllChildren => Children.Concat(Children.SelectMany(node => node.AllChildren));

        public IEnumerable<TLink> AllLinks => Links.Concat(Links.Select(LinkChild).SelectMany(node => node.AllLinks));

        public IEnumerable<(TLink link, TData data)> AllLinksWith<TData>(TData initial, Func<TData, TLink, TData> next)
        {
            return Links.Select(link => (link, initial)).Concat(Links.SelectMany(link => LinkChild(link).AllLinksWith(next(initial, link), next)));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<TLink> GetEnumerator() => Links.GetEnumerator();
    }

    public class Node<T> : Node<T, Node<T>>
    {
        public Node(T value) : base(value) { }

        public override Node<T, Node<T>> LinkChild(Node<T> link) => link;

        public new IEnumerable<Node<T>> Parents => base.Parents.Cast<Node<T>>();
    }

    public class Tree<T> : Tree<T, Node<T>, Node<T>>
    {
        public Node<T> this[T item] => GetNode(item);
        
        public Node<T> GetNode(T item)
        {
            if (TryGet(item, out var node)) return node;
            return Nodes[item] = new Node<T>(item);
        }
        
        public void Link(T parent, T child)
        {
            var p = GetNode(parent);
            var c = GetNode(child);
            p.AddChild(c);
            c.Parent = p;
        }
    }

    public static class TreeExtensions
    {
        public static Tree<TT> ToTree<T, TT>(this IEnumerable<T> items, Func<T, TT> parent, Func<T, TT> child)
        {
            var tree = new Tree<TT>();
            foreach (var item in items)
            {
                tree.Link(parent(item), child(item));
            }
            return tree;
        }

        public static Tree<TT> ToTree<T, TI, TT>(this IEnumerable<T> items, Func<T, TI> func, Func<TI, TT> parent, Func<TI, TT> child)
        {
            var tree = new Tree<TT>();
            foreach (var item in items)
            {
                var i = func(item);
                tree.Link(parent(i), child(i));
            }
            return tree;
        }

        public static IEnumerable<T> Values<T, TLink>(this IEnumerable<Node<T, TLink>> nodes)
        {
            return nodes.Select(node => node.Value);
        }

        public static Node<T> CommonAncestor<T>(this Tree<T> tree, T a, T b)
        {
            if (!tree.TryGet(a, out var an) || !tree.TryGet(b, out var bn)) return null;
            var seen = new HashSet<T>(an.Parents.Values());
            return bn.Parents.FirstOrDefault(node => seen.Contains(node.Value));
        }
    }
}