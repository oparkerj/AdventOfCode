namespace AdventToolkit2.Space.Interface;

/// <summary>
/// A type that can summarize its data using a bound.
/// </summary>
/// <typeparam name="TPos"></typeparam>
/// <typeparam name="TBound"></typeparam>
public interface IBounded<TPos, out TBound>
    where TBound : IBound<TBound, TPos>
{
    TBound Bounds { get; }
}