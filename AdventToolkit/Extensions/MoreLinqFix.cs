using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace AdventToolkit.Extensions;

public static class MoreLinqFix
{
    public static IExtremaEnumerable<T> MinsBy<T, TC>(this IEnumerable<T> items, Func<T, TC> comp)
    {
        return MoreEnumerable.MinBy(items, comp);
    }
    
    public static IExtremaEnumerable<T> MinsBy<T, TC>(this IEnumerable<T> items, Func<T, TC> comp, IComparer<TC> comparer)
    {
        return MoreEnumerable.MinBy(items, comp, comparer);
    }

    public static IExtremaEnumerable<T> MaxsBy<T, TC>(this IEnumerable<T> items, Func<T, TC> comp)
    {
        return MoreEnumerable.MaxBy(items, comp);
    }
    
    public static IExtremaEnumerable<T> MaxsBy<T, TC>(this IEnumerable<T> items, Func<T, TC> comp, IComparer<TC> comparer)
    {
        return MoreEnumerable.MaxBy(items, comp, comparer);
    }

    public static HashSet<T> ToSet<T>(this IEnumerable<T> items)
    {
        return Enumerable.ToHashSet(items);
    }

    public static IEnumerable<T> Before<T>(this IEnumerable<T> items, T item)
    {
        return Enumerable.Prepend(items, item);
    }
}