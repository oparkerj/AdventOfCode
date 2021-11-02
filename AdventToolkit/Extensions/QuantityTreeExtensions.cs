using System;
using System.Collections.Generic;
using AdventToolkit.Collections;
using RegExtract;

namespace AdventToolkit.Extensions
{
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

        // Create a quantity tree from a format string.
        // The format string is a regex that must contain the following groups:
        // Named group <Value>: the parent node
        // Named group <Children>: Group quantified by * or + that contains the children
        //      this group must contain two groups that represent the quantity and name
        //      of the child node.
        // (Optional) Named group <Amount>: The number of parent objects produced.
        public static QuantityTree<string> ToQuantityTree(this IEnumerable<string> items, string format)
        {
            return items.Extract<QuantityItem<string>>(format)
                .ToQuantityTree<QuantityItem<string>, string>((item, helper) =>
                {
                    helper.Add(item.Value, item.Amount);
                    foreach (var (amount, child) in item.Children)
                    {
                        helper.AddChild(child, amount);
                    }
                });
        }
        
        // Same as ToQuantityTree but children nodes do not have an amount.
        public static QuantityTree<string> ToWeightedTree(this IEnumerable<string> items, string format)
        {
            return items.Extract<WeightedItem<string>>(format)
                .ToQuantityTree<WeightedItem<string>, string>((item, helper) =>
                {
                    helper.Add(item.Value, item.Amount);
                    foreach (var child in item.Children)
                    {
                        helper.AddChild(child, 1);
                    }
                });
        }
    }
}