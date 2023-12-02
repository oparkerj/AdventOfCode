using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> cons)
    {
        if (dictionary.TryGetValue(key, out var value)) return value;
        return dictionary[key] = cons();
    }
    
    public static TValue GetOrNew<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
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
}