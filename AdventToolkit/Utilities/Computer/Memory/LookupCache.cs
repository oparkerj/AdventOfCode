using System;
using System.Collections.Generic;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Memory;

public class LookupCache<TArch, TKey> : IMemory<TArch>
{
    public readonly Dictionary<TKey, TArch> Values;
    public readonly Func<TKey, TArch> Lookup;

    public LookupCache(Func<TKey, TArch> lookup = null)
    {
        Values = new Dictionary<TKey, TArch>();
        Lookup = lookup;
    }

    public TArch Get<T>(T t)
    {
        if (t is TKey key)
        {
            if (Values.TryGetValue(key, out var result)) return result;
            if (Lookup == null) throw new Exception($"No value present for the key \"{key}\"");
            return Values[key] = Lookup(key);
        }
        throw new Exception("Invalid key type.");
    }

    public void Set<T>(T t, TArch value)
    {
        if (t is TKey key) Values[key] = value;
        else throw new Exception("Invalid key type.");
    }

    public void Reset() => Values.Clear();

    public TArch this[TArch t]
    {
        get => Get(t);
        set => Set(t, value);
    }

    public TArch this[int i]
    {
        get => Get(i);
        set => Set(i, value);
    }

    public TArch this[char c]
    {
        get => Get(c);
        set => Set(c, value);
    }

    public TArch this[long l]
    {
        get => Get(l);
        set => Set(l, value);
    }

    public TArch this[string s]
    {
        get => Get(s);
        set => Set(s, value);
    }
}