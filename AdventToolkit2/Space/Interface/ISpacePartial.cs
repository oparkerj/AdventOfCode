namespace AdventToolkit2.Space.Interface;

/// <summary>
/// A space which provides the ability to get positions within a bound.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TBound"></typeparam>
public interface ISpacePartial<out TPos, in TBound>
    where TBound : IBound<TBound, TPos>
{
    /// <summary>
    /// Get positions in the space within the given bound.
    /// </summary>
    /// <param name="bound"></param>
    /// <returns></returns>
    IEnumerable<TPos> PositionsIn(TBound bound);
}