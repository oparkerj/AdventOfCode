namespace AdventToolkit2.Util;

/// <summary>
/// Methods to help internal implementations.
/// </summary>
internal static class Impl
{
    /// <summary>
    /// Returns the default boolean value.
    /// </summary>
    /// <returns></returns>
    public static bool Default() => default;

    /// <summary>
    /// Sets all parameters to default and returns false.
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>false</returns>
    public static bool Default<T>(out T t)
    {
        t = default!;
        return default;
    }
    
    /// <inheritdoc cref="Default{T}"/>
    public static bool Default<T1, T2>(out T1 t1, out T2 t2)
    {
        t1 = default!;
        t2 = default!;
        return default;
    }
    
    /// <inheritdoc cref="Default{T}"/>
    public static bool Default<T1, T2, T3>(out T1 t1, out T2 t2, out T3 t3)
    {
        t1 = default!;
        t2 = default!;
        t3 = default!;
        return default;
    }
    
    /// <inheritdoc cref="Default{T}"/>
    public static bool Default<T1, T2, T3, T4>(out T1 t1, out T2 t2, out T3 t3, out T4 t4)
    {
        t1 = default!;
        t2 = default!;
        t3 = default!;
        t4 = default!;
        return default;
    }
}