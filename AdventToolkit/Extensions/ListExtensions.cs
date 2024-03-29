using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions;

public static class ListExtensions
{
    public static IEnumerable<T> Get<T>(this IList<T> t, IEnumerable<int> indices)
    {
        return indices.Select(i => t[i]);
    }

    public static IEnumerable<(T, T)> Pairs<T>(this IList<T> source)
    {
        return Algorithms.SequencesIncreasing(2, source.Count, true).Select(pair => (source[pair[0]], source[pair[1]]));
    }

    // public static IEnumerable<IEnumerable<T>> Subsets<T>(this IList<T> source, int length)
    // {
    //     return Algorithms.SequencesIncreasing(length, source.Count, true).Select(source.Get);
    // }

    public static LinkedListNode<T> NextCircular<T>(this LinkedListNode<T> node)
    {
        return node.Next ?? node.List?.First;
    }

    public static LinkedListNode<T> PreviousCircular<T>(this LinkedListNode<T> node)
    {
        return node.Previous ?? node.List?.Last;
    }
        
    // Removes elements that meet a condition while a list is being iterated; adjusts the
    // iteration index so that the loop will continue in the right spot after elements are removed.
    // Returns the number of elements removed.
    public static int RemoveConcurrent<T>(this List<T> list, Func<T, bool> cond, ref int index)
    {
        var removed = 0;
        for (var i = 0; i < list.Count; i++)
        {
            if (cond(list[i]))
            {
                removed++;
                if (i <= index) index--;
                list.RemoveAt(i--);
            }
        }
        return removed;
    }

    // This will only remove the value if it is found before the current index
    public static bool RemoveConcurrent<T>(this List<T> list, T t, ref int index)
    {
        var i = list.IndexOf(t);
        if (i < 0 || i > index) return i >= 0;
        index--;
        list.RemoveAt(i);
        return true;
    }

    // Removes the item at the current index
    public static bool RemoveConcurrent<T>(this List<T> list, ref int index)
    {
        if (index < 0 || index >= list.Count) return false;
        list.RemoveAt(index--);
        return true;
    }

    // For each pair of elements in the input, groups ones which meet a condition.
    // If A,B and B,C both meet the conditions, A, B, and C will be in the same group.
    // The integer key in the result is arbitrary and only used to mark groups.
    public static ILookup<int, T> GroupPairs<T>(this IList<T> items, Func<T, T, bool> predicate)
    {
        var count = 0;
        var groups = new Dictionary<T, int>();

        int Group(T t) => groups.TryGetValue(t, out var g) ? g : groups[t] = count++;

        void SetGroup(T t, int group)
        {
            if (!groups.TryGetValue(t, out var current)) groups[t] = group;
            else if (current != group)
            {
                foreach (var (item, _) in groups.WhereValue(current).ToList())
                {
                    groups[item] = group;
                }
            }
        }

        foreach (var (a, b) in items.Pairs())
        {
            var g = Group(a);
            if (predicate(a, b)) SetGroup(b, g);
            else Group(b);
        }

        return items.ToLookup(t => groups[t]);
    }

    public static IEnumerable<LinkedListNode<T>> Nodes<T>(this LinkedList<T> list)
    {
        var node = list.First;
        while (node != null)
        {
            yield return node;
            node = node.Next;
        }
    }

    public static LinkedListNode<T> GetNode<T>(this LinkedList<T> list, int index)
    {
        if (index < 0) throw new ArgumentException("Invalid index");
        var node = list.First;
        while (index-- > 0 && node != null)
        {
            node = node.Next;
        }
        return node;
    }

    public static List<T> ToList<T>(this IEnumerable<T> items, List<T> dest, bool clear = true)
    {
        if (clear) dest.Clear();
        dest.AddRange(items);
        return dest;
    }

    public static IList<T> AsList<T>(this IEnumerable<T> items)
    {
        return items as IList<T> ?? items.ToList();
    }

    public static T[] AsArray<T>(this IEnumerable<T> items)
    {
        return items is ICollection<T> col ? col as T[] ?? items.ToArray(col.Count) : items.ToArray();
    }
}