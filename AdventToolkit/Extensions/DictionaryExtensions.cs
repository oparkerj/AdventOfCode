using System;
using System.Collections.Generic;

namespace AdventToolkit.Extensions;

public static class DictionaryExtensions
{
    public static TV GetDefault<TK, TV>(this Dictionary<TK, TV> d, TK key, Func<TV> def)
    {
        return !d.TryGetValue(key, out var result) ? d[key] = def() : result;
    }
    
    public static TV GetDefault<TK, TV>(this Dictionary<TK, TV> d, TK key, TV def)
    {
        return d.GetDefault(key, () => def);
    }
}