namespace AdventToolkit.New.Interface;

/// <summary>
/// Represents a set of values defined by a minimum and maximum value.
/// </summary>
/// <typeparam name="T">Bound type.</typeparam>
/// <typeparam name="TNum">Bound value type.</typeparam>
public interface IBound<T, TNum> : IEnumerable<TNum>
    where T : IBound<T, TNum>
{
    /// <summary>
    /// Create a bound that spans the given values.
    /// This is inclusive on both sides.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static abstract T Span(TNum a, TNum b);

    /// <summary>
    /// Create a bound from start (inclusive) to end (exclusive).
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    static abstract T From(TNum start, TNum end);
    
    static abstract T Empty { get; }
    
    /// <summary>
    /// Minimum value in the bound.
    /// </summary>
    TNum Min { get; }
    
    /// <summary>
    /// Maximum value in the bound. (inclusive maximum)
    /// </summary>
    TNum Max { get; }
    
    /// <summary>
    /// Ending value of the bound. (exclusive maximum)
    /// </summary>
    TNum End { get; }

    /// <summary>
    /// Get the size (dimensions) of the bound.
    /// </summary>
    TNum Size { get; }

    /// <summary>
    /// Check if a value is contained in the bound.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    bool Contains(TNum t);

    /// <summary>
    /// Get the intersection of this bound and another.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    T Intersect(T other);
}