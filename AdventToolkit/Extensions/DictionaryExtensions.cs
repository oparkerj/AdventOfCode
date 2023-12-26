using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventToolkit.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> cons)
    {
        if (dictionary.TryGetValue(key, out var value)) return value;
        return dictionary[key] = cons();
    }
    
    public static TValue GetOrNew<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        if (dictionary.TryGetValue(key, out var value)) return value;
        return dictionary[key] = new TValue();
    }

    public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue value = default)
    {
        return key is null ? value : dictionary.GetValueOrDefault(key, value);
    }

    public static IEnumerable<TValue> GetValues<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
    {
        return keys.Select(key => dictionary[key]);
    }

    public static void UpdateMany<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        foreach (var (key, value) in pairs)
        {
            dictionary[key] = value;
        }
    }

    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs, Func<TKey, TValue, TValue, TValue> update)
    {
        foreach (var (key, newValue) in pairs)
        {
            if (!dictionary.TryGetValue(key, out var oldValue))
            {
                dictionary[key] = newValue;
            }
            else
            {
                dictionary[key] = update(key, oldValue, newValue);
            }
        }
    }
    
    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> pairs, Func<TValue, TValue, TValue> update)
    {
        foreach (var (key, newValue) in pairs)
        {
            if (!dictionary.TryGetValue(key, out var oldValue))
            {
                dictionary[key] = newValue;
            }
            else
            {
                dictionary[key] = update(oldValue, newValue);
            }
        }
    }
    
    public static bool Compare<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right, Func<TValue, TValue, bool> comp)
    {
        foreach (var (key, leftValue) in left)
        {
            if (!right.TryGetValue(key, out var rightValue)) return false;
            if (!comp(leftValue, rightValue)) return false;
        }
        return true;
    }

    public static bool Lt<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        return left.Compare(right, (l, r) => l < r);
    }
    
    public static bool Le<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        return left.Compare(right, (l, r) => l <= r);
    }
    
    public static bool Gt<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        return left.Compare(right, (l, r) => l > r);
    }
    
    public static bool Ge<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        where TValue : IComparisonOperators<TValue, TValue, bool>
    {
        return left.Compare(right, (l, r) => l >= r);
    }
}