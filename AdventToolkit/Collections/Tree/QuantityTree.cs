using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Graph;

namespace AdventToolkit.Collections.Tree;

public class QuantityTree<T> : Tree<T, QuantityVertex<T>, DataEdge<T, long>>
{
    public long TotalVertexWeight(QuantityVertex<T> vertex)
    {
        return Bfs(vertex).Select(v => v.Quantity).Sum();
        // var total = vertex.Quantity;
        // var next = new Queue<QuantityVertex<T>>(vertex.Neighbors);
        // while (next.Count > 0)
        // {
        //     var v = next.Dequeue();
        //     total += v.Quantity;
        //     v.Neighbors.ForEach(next.Enqueue);
        // }
        // return total;
        // return vertex.Quantity + vertex.Descendants.Cast<QuantityVertex<T>>().Select(v => v.Quantity).Sum();
    }
        
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
        foreach (var (child, amount) in vertex.Produced())
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

public class QuantityVertex<T> : TreeVertex<T, DataEdge<T, long>>
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

    public new IEnumerable<QuantityVertex<T>> Neighbors => base.Neighbors.Cast<QuantityVertex<T>>();

    public override string ToString()
    {
        return $"\"{Value} ({Quantity})\"";
    }
}

public class QuantityTreeHelper<T>
{
    public readonly QuantityTree<T> Tree;
    private QuantityVertex<T> _parent;

    public QuantityTreeHelper(QuantityTree<T> tree) => Tree = tree;

    public QuantityVertex<T> GetOrCreate(T item)
    {
        if (Tree.TryGet(item, out var vertex)) return vertex;
        vertex = new QuantityVertex<T>(item, 0);
        Tree.AddVertex(vertex);
        return vertex;
    }
        
    public QuantityVertex<T> GetOrCreate(T item, long amount)
    {
        if (Tree.TryGet(item, out var vertex))
        {
            vertex.Quantity = amount;
            return vertex;
        }
        vertex = new QuantityVertex<T>(item, amount);
        Tree.AddVertex(vertex);
        return vertex;
    }

    public QuantityTreeHelper<T> Add(T item, long amount = 1)
    {
        _parent = GetOrCreate(item, amount);
        return this;
    }

    public QuantityTreeHelper<T> AddChild(T item, long amount)
    {
        var node = GetOrCreate(item);
        _parent.LinkTo(node, new DataEdge<T, long>(_parent, node, amount));
        return this;
    }
}