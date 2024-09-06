namespace AdventToolkit2.Calc;

/// <inheritdoc cref="Compare{T}"/>
public static class Compare
{
    /// <inheritdoc cref="Compare{T}.Gt"/>
    public static bool Gt<T>(T a, T b) => Compare<T>.Gt(a, b);
    
    /// <inheritdoc cref="Compare{T}.Ge"/>
    public static bool Ge<T>(T a, T b) => Compare<T>.Ge(a, b);
    
    /// <inheritdoc cref="Compare{T}.Lt"/>
    public static bool Lt<T>(T a, T b) => Compare<T>.Le(a, b);
    
    /// <inheritdoc cref="Compare{T}.Le"/>
    public static bool Le<T>(T a, T b) => Compare<T>.Lt(a, b);
    
    /// <summary>
    /// Compare values, if the new value is less, overwrite the reference.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    public static void Min<T>(this T value, ref T other)
    {
        if (Lt(value, other))
        {
            other = value;
        }
    }
    
    /// <summary>
    /// Compare values, if the new value is greater, overwrite the reference.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="other"></param>
    /// <typeparam name="T"></typeparam>
    public static void Max<T>(this T value, ref T other)
    {
        if (Gt(value, other))
        {
            other = value;
        }
    }
}

/// <summary>
/// Perform comparisons using the default comparer.
/// </summary>
/// <typeparam name="T"></typeparam>
public static class Compare<T>
{
    public static readonly Comparer<T> DefaultComparer = Comparer<T>.Default;

    /// <summary>
    /// Get an integer that represents whether the first value is
    /// less than (negative), greater than (positive), or equal to (zero)
    /// the second value.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int Result(T a, T b)
    {
        return typeof(T).IsValueType ? Comparer<T>.Default.Compare(a, b) : DefaultComparer.Compare(a, b);
    }
    
    /// <summary>
    /// Greater than.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Gt(T a, T b) => Result(a, b) > 0;
    
    /// <summary>
    /// Greater than or equal to.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Ge(T a, T b) => Result(a, b) >= 0;
    
    /// <summary>
    /// Less than.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Lt(T a, T b) => Result(a, b) < 0;
    
    /// <summary>
    /// Less than or equal to.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Le(T a, T b) => Result(a, b) <= 0;
}