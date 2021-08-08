using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public class Tree<T> : IEnumerable<Tree<T>.Node<T>>
    {
        private readonly Dictionary<T, Node<T>> _nodes = new();

        public Node<T> this[T item] => GetNode(item);

        public bool TryGet(T item, out Node<T> node)
        {
            return _nodes.TryGetValue(item, out node);
        }

        public Node<T> GetNode(T item)
        {
            if (TryGet(item, out var node)) return node;
            return _nodes[item] = new Node<T>(item);
        }

        public void Link(T parent, T child)
        {
            var p = GetNode(parent);
            var c = GetNode(child);
            p.Children.Add(c);
            c.Parent = p;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<Node<T>> GetEnumerator() => _nodes.Values.GetEnumerator();

        public class Node<TN> : IEnumerable<Node<TN>>
        {
            public Node<TN> Parent;
            public readonly List<Node<TN>> Children = new(2);
            
            public readonly TN Value;

            public Node(TN value) => Value = value;

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

            public IEnumerable<Node<TN>> Parents
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

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<Node<TN>> GetEnumerator() => Children.GetEnumerator();
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

        public static IEnumerable<T> Values<T>(this IEnumerable<Tree<T>.Node<T>> nodes)
        {
            return nodes.Select(node => node.Value);
        }

        public static Tree<T>.Node<T> CommonAncestor<T>(this Tree<T> tree, T a, T b)
        {
            if (!tree.TryGet(a, out var an) || !tree.TryGet(b, out var bn)) return null;
            var seen = new HashSet<T>(an.Parents.Values());
            return bn.Parents.FirstOrDefault(node => seen.Contains(node.Value));
        }
    }
}