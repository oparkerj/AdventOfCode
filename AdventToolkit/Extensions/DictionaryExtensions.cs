using System;
using System.Collections.Generic;

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
}