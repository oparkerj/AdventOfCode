namespace AdventToolkit.New.Interface;

/// <summary>
/// A set of values of the given position type.
/// In this space, the adjacent values for any position is well defined.
/// </summary>
/// <typeparam name="TPos"></typeparam>
public interface ISpace<TPos>
{
    /// <summary>
    /// Get positions which are adjacent to another position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    IEnumerable<TPos> GetNeighbors(TPos pos);
}

/// <summary>
/// A space where positions can be mapped to values.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
public interface ISpace<TPos, TVal> : ISpace<TPos>
{
    /// <summary>
    /// Number of mapped positions.
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Default value.
    /// </summary>
    public TVal Default { get; set; }
    
    /// <summary>
    /// Add a position mapping.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="val"></param>
    void Add(TPos pos, TVal val);

    /// <summary>
    /// Remove a position mapping.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool Remove(TPos pos);

    /// <summary>
    /// Try to get a position mapping.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="val"></param>
    /// <returns>True if the position is mapped, false otherwise.</returns>
    bool TryGet(TPos pos, out TVal val);

    /// <summary>
    /// Get the mapped value for a position, returns a default value if the position
    /// is not mapped.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    TVal Get(TPos pos);

    /// <summary>
    /// Get the mapped value for a position, throwing an exception if the
    /// position is not mapped.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    TVal GetStrict(TPos pos) => this[pos];
    
    /// <summary>
    /// Get/Set position mappings. This throws an exception if the position is
    /// not mapped.
    /// </summary>
    /// <param name="pos"></param>
    TVal this[TPos pos] { get; set; }

    /// <summary>
    /// Check if a position has a mapping.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool Contains(TPos pos);

    /// <summary>
    /// Check if the given value is mapped from any position.
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    bool ContainsValue(TVal val);

    /// <summary>
    /// Remove all mappings.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Enumerate all mapped positions.
    /// </summary>
    IEnumerable<TPos> Positions { get; }
    
    /// <summary>
    /// Enumerate all mapped values.
    /// </summary>
    IEnumerable<TVal> Values { get; }

    /// <summary>
    /// Add a mapping only if the position is not mapped yet.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="val"></param>
    /// <returns>The existing mapped value for the position, or the
    /// newly mapped value.</returns>
    TVal AddDefault(TPos pos, TVal val)
    {
        if (TryGet(pos, out var existing)) return existing;
        return this[pos] = val;
    }
}