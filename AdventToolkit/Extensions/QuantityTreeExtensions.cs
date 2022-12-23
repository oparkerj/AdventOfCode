using System;
using System.Collections.Generic;
using System.Numerics;
using AdventToolkit.Collections.Graph;
using AdventToolkit.Collections.Tree;
using RegExtract;

namespace AdventToolkit.Extensions;

public static class QuantityTreeExtensions
{
    public static QuantityTree<TResult, TNum> ToQuantityTree<T, TResult, TNum>(this IEnumerable<T> items, Action<T, QuantityTreeHelper<TResult, TNum>> action)
        where TNum : INumber<TNum>
    {
        var tree = new QuantityTree<TResult, TNum>();
        var helper = new QuantityTreeHelper<TResult, TNum>(tree);
        foreach (var item in items)
        {
            action(item, helper);
        }
        return tree;
    }
        
    public static QuantityTree<TResult, TNum> ToQuantityTree<T, TI, TResult, TNum>(this IEnumerable<T> items, Func<T, TI> func, Action<TI, QuantityTreeHelper<TResult, TNum>> action)
        where TNum : INumber<TNum>
    {
        var tree = new QuantityTree<TResult, TNum>();
        var helper = new QuantityTreeHelper<TResult, TNum>(tree);
        foreach (var item in items)
        {
            action(func(item), helper);
        }
        return tree;
    }

    // Create a quantity tree from a format string.
    // The format string is a regex that must contain the following groups:
    // Named group <Value>: the parent node
    // Named group <Children>: Group quantified by * or + that contains the children
    //      this group must contain two groups that represent the quantity and name
    //      of the child node.
    // (Optional) Named group <Amount>: The number of parent objects produced.
    public static QuantityTree<string, TNum> ToQuantityTree<TNum>(this IEnumerable<string> items, string format)
        where TNum : INumber<TNum>
    {
        return items.Extract<QuantityVertexInfo<string, TNum>>(format)
            .ToQuantityTree<QuantityVertexInfo<string, TNum>, string, TNum>((item, helper) =>
            {
                helper.Add(item.Value, item.Amount);
                if (item.Child != null)
                {
                    helper.AddChild(item.Child, item.ChildAmount);
                }
                else if (item.Children != null)
                {
                    foreach (var v in item.Children)
                    {
                        helper.AddChild(v, TNum.One);
                    }
                }
                else if (item.ChildWeight != null)
                {
                    foreach (var (child, amount) in item.ChildWeight)
                    {
                        helper.AddChild(child, amount);
                    }
                }
                else if (item.WeightChild != null)
                {
                    foreach (var (amount, child) in item.WeightChild)
                    {
                        helper.AddChild(child, amount);
                    }
                }
            });
    }
}