using AdventToolkit2.Space.Interface;

namespace AdventToolkit2.Space;

public static class SpaceExtensions
{
    /// <summary>
    /// Get a key from a space or return a default value if it is not present.
    /// </summary>
    /// <param name="space"></param>
    /// <param name="key"></param>
    /// <param name="default"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVal"></typeparam>
    /// <returns></returns>
    public static TVal Get<TKey, TVal>(this ISpace<TKey, TVal> space, TKey key, TVal @default)
    {
        return space.TryGet(key, out var value) ? value : @default;
    }
}