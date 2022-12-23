using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Tree;

public class QuantityTree<T, TNum> : Tree<T, QuantityVertex<T, TNum>, DataEdge<T, TNum>>
    where TNum : INumber<TNum>
{
    public TNum TotalVertexWeight(QuantityVertex<T, TNum> vertex)
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

    // Find how much item can be produced using a number of source items.
    // Finds the answer by seeing how many source are used to produce one
    // item and using that to quickly converge to the result.
    public TNum ProduceUsing(T item, T source, TNum amount)
    {
        var last = TNum.Zero;
        var estimate = TNum.One;
        var unit = -TNum.One;
        while (true)
        {
            var made = Produce(item, estimate)[source];
            if (unit == -TNum.One) unit = made;
            if (made == amount) return estimate;
            if (made < amount)
            {
                last = estimate;
                estimate += TNum.Max((amount - made) / unit, TNum.One);
            }
            else if (made > amount)
            {
                return last;
            }
        }
    }
        
    public Dictionary<T, TNum> Produce(T item, TNum quantity)
    {
        return Produce(item, quantity, out _);
    }

    public Dictionary<T, TNum> Produce(T item, TNum quantity, out DefaultDict<T, TNum> extra)
    {
        extra = new DefaultDict<T, TNum>();
        var count = new DefaultDict<T, TNum>();
        Produce(item, quantity, count, extra);
        return count;
    }

    private void Produce(T item, TNum quantity, DefaultDict<T, TNum> count, DefaultDict<T, TNum> extra)
    {
        if (!TryGet(item, out var vertex)) throw new Exception("Cannot produce needed item.");
        TNum willProduce;
        var scale = TNum.Zero;
        if (vertex.Quantity == TNum.Zero && vertex.Count != 0) throw new Exception("Only 0 of the item can be made.");
        if (vertex.Quantity == TNum.Zero) willProduce = quantity;
        else
        {
            willProduce = quantity % vertex.Quantity == TNum.Zero ? quantity : quantity + (vertex.Quantity - (quantity % vertex.Quantity));
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

public class QuantityVertexBase<T, TEdge, TNum> : TreeVertex<T, TEdge>
    where TEdge : Edge<T>
    where TNum : INumber<TNum>
{
    public TNum Quantity;

    public QuantityVertexBase(T value, TNum quantity) : base(value)
    {
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"\"{Value} ({Quantity})\"";
    }
}

public class QuantityVertex<T, TNum> : QuantityVertexBase<T, DataEdge<T, TNum>, TNum>
    where TNum : INumber<TNum>
{
    public QuantityVertex(T value, TNum quantity) : base(value, quantity) { }

    public TNum SumBranches()
    {
        return DescendantLinks.Select(edge => edge.Data).Sum();
    }

    public IEnumerable<(QuantityVertex<T, TNum>, TNum)> Produced()
    {
        return NeighborEdges.Select(edge => (edge.OtherAs(this), edge.Data));
    }

    public new IEnumerable<QuantityVertex<T, TNum>> Neighbors => base.Neighbors.Cast<QuantityVertex<T, TNum>>();
}

public class QuantityTreeHelper<T, TNum>
    where TNum : INumber<TNum>
{
    public readonly QuantityTree<T, TNum> Tree;
    private QuantityVertex<T, TNum> _parent;

    public QuantityTreeHelper(QuantityTree<T, TNum> tree) => Tree = tree;

    public QuantityVertex<T, TNum> GetOrCreate(T item)
    {
        if (Tree.TryGet(item, out var vertex)) return vertex;
        vertex = new QuantityVertex<T, TNum>(item, TNum.Zero);
        Tree.AddVertex(vertex);
        return vertex;
    }
        
    public QuantityVertex<T, TNum> GetOrCreate(T item, TNum amount)
    {
        if (Tree.TryGet(item, out var vertex))
        {
            vertex.Quantity = amount;
            return vertex;
        }
        vertex = new QuantityVertex<T, TNum>(item, amount);
        Tree.AddVertex(vertex);
        return vertex;
    }

    public QuantityTreeHelper<T, TNum> Add(T item, TNum amount)
    {
        _parent = GetOrCreate(item, amount);
        return this;
    }

    public QuantityTreeHelper<T, TNum> AddChild(T item, TNum amount)
    {
        var node = GetOrCreate(item);
        _parent.LinkTo(node, new DataEdge<T, TNum>(_parent, node, amount));
        return this;
    }
}