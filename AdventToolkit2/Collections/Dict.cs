using AdventToolkit2.Space.Set;

namespace AdventToolkit2.Collections;

/// <summary>
/// Dict is effectively an alias for SparseSpace with no specialization,
/// which behaves like a Dictionary but with default values.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class Dict<TKey, TValue> : SparseSpace<TKey, TValue>
    where TKey : notnull;