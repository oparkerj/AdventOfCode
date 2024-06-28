namespace AdventToolkit.New.Calc;

/// <inheritdoc cref="Equality{T}"/>
public static class Equality
{
    /// <inheritdoc cref="Equality{T}.Eq"/>
    public static bool Eq<T>(T a, T b) => Equality<T>.Eq(a, b);
    
    /// <inheritdoc cref="Equality{T}.Ne"/>
    public static bool Ne<T>(T a, T b) => Equality<T>.Ne(a, b);
}

/// <summary>
/// Check for equality using the default comparer.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class Equality<T>
{
    public static readonly EqualityComparer<T> EqualityComparer = EqualityComparer<T>.Default;
    
    /// <summary>
    /// Get the equality result.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Result(T a, T b)
    {
        return typeof(T).IsValueType ? EqualityComparer<T>.Default.Equals(a, b) : EqualityComparer.Equals(a, b);
    }
    
    /// <summary>
    /// Equal to.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Eq(T a, T b) => Result(a, b);
    
    /// <summary>
    /// Not equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Ne(T a, T b) => !Result(a, b);
}