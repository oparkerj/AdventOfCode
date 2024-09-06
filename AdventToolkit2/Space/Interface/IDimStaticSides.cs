namespace AdventToolkit2.Space.Interface;

/// <summary>
/// An implementation of <see cref="IDimSides{T,TSide}"/> where the methods can be
/// implemented statically.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSide"></typeparam>
public interface IDimStaticSides<TSelf, T, TSide> : IDimSides<T, TSide>
    where TSelf : IDimStaticSides<TSelf, T, TSide>
{
    IEnumerable<TSide> IDimSides<T, TSide>.GetSides(T pos) => TSelf.GetSidesStatic(pos);

    T IDimSides<T, TSide>.GetSide(T pos, TSide side) => TSelf.GetSideStatic(pos, side);

    bool IDimSides<T, TSide>.TryLookupSide(T pos, T other, out TSide side)
    {
        return TSelf.TryLookupSideStatic(pos, other, out side);
    }

    bool IDimSides<T, TSide>.TryLookupSideExact(T pos, T other, out TSide side)
    {
        return TSelf.TryLookupSideExactStatic(pos, other, out side);
    }

    /// <inheritdoc cref="IDimSides{T,TSide}.GetSides"/>
    static abstract IEnumerable<TSide> GetSidesStatic(T pos);

    /// <inheritdoc cref="IDimSides{T,TSide}.GetSide"/>
    static abstract T GetSideStatic(T pos, TSide side);

    /// <inheritdoc cref="IDimSides{T,TSide}.TryLookupSide"/>
    static abstract bool TryLookupSideStatic(T pos, T other, out TSide side);

    /// <inheritdoc cref="IDimSides{T,TSide}.TryLookupSideExact"/>
    static abstract bool TryLookupSideExactStatic(T pos, T other, out TSide side);
}