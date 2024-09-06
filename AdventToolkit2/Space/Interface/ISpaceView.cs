namespace AdventToolkit2.Space.Interface;

/// <summary>
/// Represents a view over a space.
/// Provides the ability to view the space over a given bound.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TVal"></typeparam>
/// <typeparam name="TSlice"></typeparam>
/// <typeparam name="TBound"></typeparam>
public interface ISpaceView<TPos, TVal, out TSlice, TBound> : IAlignedSpace<TPos, TVal>, IBounded<TPos, TBound>
    where TSlice : ISpaceView<TPos, TVal, TSlice, TBound>
    where TBound : IBound<TBound, TPos>
{
    /// <summary>
    /// Get a view of the space over a bound.
    /// </summary>
    /// <param name="bound"></param>
    /// <returns></returns>
    TSlice View(TBound bound);
}