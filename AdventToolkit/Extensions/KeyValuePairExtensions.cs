using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions;

public static class KeyValuePairExtensions
{
    public static IEnumerable<KeyValuePair<TValue, TKey>> Swap<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        return source.Select(pair => new KeyValuePair<TValue, TKey>(pair.Value, pair.Key));
    }
        
    public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        return source.Select(pair => pair.Key);
    }
        
    public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        return source.Select(pair => pair.Value);
    }

    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
    {
        return source.Where(pair => Equals(pair.Key, key));
    }
        
    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TKey, bool> func)
    {
        return source.Where(pair => func(pair.Key));
    }
        
    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TValue value)
    {
        return source.Where(pair => Equals(pair.Value, value));
    }
        
    public static IEnumerable<KeyValuePair<TKey, TValue>> WhereValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TValue, bool> func)
    {
        return source.Where(pair => func(pair.Value));
    }

    public static void ForEach<TA, TB>(this IEnumerable<KeyValuePair<TA, TB>> items, Action<TA, TB> action)
    {
        foreach (var (a, b) in items)
        {
            action(a, b);
        }
    }

    public static IOrderedEnumerable<KeyValuePair<TKey, TValue>> OrderByKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        return pairs.OrderBy(pair => pair.Key);
    }
        
    public static IOrderedEnumerable<KeyValuePair<TKey, TValue>> OrderByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        return pairs.OrderBy(pair => pair.Value);
    }
}