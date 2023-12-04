using System.Collections.Generic;

namespace AdventToolkit.Common;

public readonly record struct Pair<TKey, TValue>(TKey Key, TValue Value)
{
    public static implicit operator Pair<TKey, TValue>(KeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);

    public static implicit operator Pair<TKey, TValue>((TKey, TValue) tuple) => new(tuple.Item1, tuple.Item2);
    
    public static implicit operator KeyValuePair<TKey, TValue>(Pair<TKey, TValue> pair) => new(pair.Key, pair.Value);

    public static implicit operator (TKey, TValue)(Pair<TKey, TValue> pair) => (pair.Key, pair.Value);

    public Pair<TValue, TKey> Swap() => new(Value, Key);
}