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

    public static IExtremaEnumerable<T> MaxsBy<T, TC>(this IEnumerable<T> items, Func<T, TC> comp)
    {
        return MoreEnumerable.MaxBy(items, comp);
    }

    public static HashSet<T> ToSet<T>(this IEnumerable<T> items)
    {
        return Enumerable.ToHashSet(items);
    }
}