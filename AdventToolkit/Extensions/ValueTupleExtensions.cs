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

    public static TR Select<TA, TB, TR>(this (TA A, TB B) tuple, Func<TA, TB, TR> func)
    {
        return func(tuple.A, tuple.B);
    }

    public static (TR, TR) Select<T, TR>(this (T A, T B) tuple, Func<T, TR> func)
    {
        return (func(tuple.A), func(tuple.B));
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