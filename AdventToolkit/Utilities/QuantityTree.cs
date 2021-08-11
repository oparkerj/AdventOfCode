using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public class QuantityTree<T> : Tree<T, QuantityNode<T>, CountLink<T>>
    {
        public long ProduceFrom(T item, Dictionary<T, long> have)
        {
            var made = new DefaultDict<T, long>();
            var extra = new DefaultDict<T, long>(have);
            long count = 0;
            while (true)
            {
                Produce(item, 1, made, extra);
                if (have.Any(pair => made[pair.Key] > pair.Value)) break;
                count++;
            }
            return count;
        }

        // Find how much item can be produced using a number of source items.
        // Finds the answer by seeing how many source are used to produce one
        // item and using that to quickly converge to the result.
        public long ProduceUsing(T item, T source, long amount)
        {
            long last = 0;
            long estimate = 1;
            long unit = -1;
            while (true)
            {
                var made = Produce(item, estimate)[source];
                if (unit == -1) unit = made;
                if (made == amount) return estimate;
                if (made < amount)
                {
                    last = estimate;
                    estimate += Math.Max((amount - made) / unit, 1);
                }
                else if (made > amount)
                {
                    return last;
                }
            }
        }
        
        public Dictionary<T, long> Produce(T item, long quantity)
        {
            return Produce(item, quantity, out _);
        }

        public Dictionary<T, long> Produce(T item, long quantity, out DefaultDict<T, long> extra)
        {
            extra = new DefaultDict<T, long>();
            var count = new DefaultDict<T, long>();
            Produce(item, quantity, count, extra);
            return count;
        }

        private void Produce(T item, long quantity, DefaultDict<T, long> count, DefaultDict<T, long> extra)
        {
            if (!TryGet(item, out var node)) throw new Exception("Cannot produce needed item.");
            long willProduce;
            var scale = 1L;
            if (node.Quantity == 0 && node.Count != 0) throw new Exception("Only 0 of the item can be made.");
            if (node.Quantity == 0) willProduce = quantity;
            else
            {
                willProduce = quantity % node.Quantity == 0 ? quantity : quantity + (node.Quantity - (quantity % node.Quantity));
                scale = willProduce / node.Quantity;
            }
            foreach (var (child, amount) in node)
            {
                var want = amount * scale;
                var piece = child.Value;
                extra.TryGetValue(piece, out var have);
                if (have > want)
                {
                    count[piece] += want;
                    extra[piece] -= want;
                }
                else if (have == want)
                {
                    count[piece] += want;
                    extra.Remove(piece);
                }
                else
                {
                    count[piece] += have;
                    extra.Remove(piece);
                    Produce(piece, want - have, count, extra);
                }
            }
            count[item] += quantity;
            if (willProduce > quantity) extra[item] += willProduce - quantity;
        }
    }

    public class QuantityNode<T> : Node<T, CountLink<T>>
    {
        public long Quantity;

        public QuantityNode(T value, long quantity) : base(value) => Quantity = quantity;

        public override Node<T, CountLink<T>> LinkChild(CountLink<T> link) => link.Node;

        public void AddChild(QuantityNode<T> child, long amount)
        {
            AddChild(new CountLink<T>(child, amount));
        }
    }

    public class CountLink<T>
    {
        public readonly QuantityNode<T> Node;
        public readonly long Amount;

        public CountLink(QuantityNode<T> node, long amount)
        {
            Node = node;
            Amount = amount;
        }

        public void Deconstruct(out QuantityNode<T> node, out long amount)
        {
            node = Node;
            amount = Amount;
        }
    }

    public class QuantityTreeHelper<T>
    {
        public readonly QuantityTree<T> Tree;
        private QuantityNode<T> _parent;

        public QuantityTreeHelper(QuantityTree<T> tree) => Tree = tree;

        public QuantityNode<T> GetOrCreate(T item)
        {
            if (Tree.TryGet(item, out var node)) return node;
            node = new QuantityNode<T>(item, 0);
            Tree.Add(node);
            return node;
        }
        
        public QuantityNode<T> GetOrCreate(T item, long amount)
        {
            if (Tree.TryGet(item, out var node))
            {
                node.Quantity = amount;
                return node;
            }
            node = new QuantityNode<T>(item, amount);
            Tree.Add(node);
            return node;
        }

        public QuantityTreeHelper<T> Add(T item, long amount = 1)
        {
            _parent = GetOrCreate(item, amount);
            return this;
        }

        public QuantityTreeHelper<T> AddChild(T item, long amount)
        {
            var node = GetOrCreate(item);
            _parent.AddChild(node, amount);
            return this;
        }
    }

    public static class QuantityTreeExtensions
    {
        public static QuantityTree<TT> ToQuantityTree<T, TT>(this IEnumerable<T> items, Action<T, QuantityTreeHelper<TT>> action)
        {
            var tree = new QuantityTree<TT>();
            var helper = new QuantityTreeHelper<TT>(tree);
            foreach (var item in items)
            {
                action(item, helper);
            }
            return tree;
        }
        
        public static QuantityTree<TT> ToQuantityTree<T, TI, TT>(this IEnumerable<T> items, Func<T, TI> func, Action<TI, QuantityTreeHelper<TT>> action)
        {
            var tree = new QuantityTree<TT>();
            var helper = new QuantityTreeHelper<TT>(tree);
            foreach (var item in items)
            {
                action(func(item), helper);
            }
            return tree;
        }
    }
}