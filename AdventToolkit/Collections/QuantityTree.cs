﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventToolkit.Collections
{
    public class QuantityTree<T> : Tree<T, QuantityVertex<T>, WeightedEdge<T>>
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
        
        public Dictionary<T, long> Produce(T item, long quantity = 1)
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
            if (!TryGet(item, out var vertex)) throw new Exception("Cannot produce needed item.");
            long willProduce;
            var scale = 1L;
            if (vertex.Quantity == 0 && vertex.Count != 0) throw new Exception("Only 0 of the item can be made.");
            if (vertex.Quantity == 0) willProduce = quantity;
            else
            {
                willProduce = quantity % vertex.Quantity == 0 ? quantity : quantity + (vertex.Quantity - (quantity % vertex.Quantity));
                scale = willProduce / vertex.Quantity;
            }
            foreach (var (child, amount) in )
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

    public class QuantityVertex<T> : TreeVertex<T, WeightedEdge<T>>
    {
        public long Quantity;

        public QuantityVertex(T value, long quantity) : base(value)
        {
            Quantity = quantity;
        }

        public long SumBranches()
        {
            return DescendantLinks.Select(edge => edge.Data).Sum();
        }

        public IEnumerable<(QuantityVertex<T>, long)> Produced()
        {
            return NeighborEdges.Select(edge => (edge.OtherAs(this), edge.Data));
        }
    }
}