using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventToolkit.Extensions;

public static class ValueTupleExtensions
{
    public static T[] ToArray<T>(this ITuple tuple)
    {
        var result = new T[tuple.Length];
        for (var i = 0; i < result.Length; i++)
        {
            if (tuple[i] is T t) result[i] = t;
            else throw new ArgumentException("Tuple type does not match.");
        }
        return result;
    }

    public static IEnumerable<T> Unpack<T>(this (T, T) tuple)
    {
        yield return tuple.Item1;
        yield return tuple.Item2;
    }

    public static IEnumerable<T> UnpackAll<T>(this IEnumerable<(T, T)> tuples)
    {
        return tuples.SelectMany(tuple => tuple.Unpack());
    }

    public static (T, T) Sorted<T>(this (T a, T b) tuple, IComparer<T> comparer = default)
    {
        comparer ??= Comparer<T>.Default;
        return comparer.Compare(tuple.a, tuple.b) <= 0 ? tuple : (tuple.b, tuple.a);
    }

    public static TR Select<TA, TB, TR>(this (TA A, TB B) tuple, Func<TA, TB, TR> func)
    {
        return func(tuple.A, tuple.B);
    }

    public static (TR, TR) Select<T, TR>(this (T A, T B) tuple, Func<T, TR> func)
    {
        return (func(tuple.A), func(tuple.B));
    }

    public static IEnumerable<(TB, TB)> TupleSelect<TA, TB>(this IEnumerable<(TA, TA)> tuples, Func<TA, TB> func)
    {
        return tuples.Select(tuple => (func(tuple.Item1), func(tuple.Item2)));
    }

    public static IEnumerable<TO> SpreadSelect<TA, TB, TO>(this IEnumerable<(TA, TB)> tuples, Func<TA, TB, TO> func)
    {
        return tuples.Select(tuple => func(tuple.Item1, tuple.Item2));
    }

    public static IEnumerable<TA> TupleFirst<TA, TB>(this IEnumerable<(TA, TB)> items)
    {
        return items.Select(tuple => tuple.Item1);
    }
    
    public static IEnumerable<TB> TupleSecond<TA, TB>(this IEnumerable<(TA, TB)> items)
    {
        return items.Select(tuple => tuple.Item2);
    }
}