using AdventToolkit.New.Space.Bound;

namespace AdventToolkit.New.Collections.Interface;

/// <summary>
/// Interface to a slice of a 2d array.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSlice"></typeparam>
public interface IArray2dSlice<T, out TSlice> : IArray2d<T>
    where TSlice : IArray2d<T>
{
    /// <summary>
    /// 2d slice indexer
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    TSlice this[Interval<int> x, Interval<int> y] { get; }
    
    /// <summary>
    /// 2d slice indexer for range inputs
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    TSlice this[Range x, Range y] { get; }
    
    /// <summary>
    /// 2d slice indexer on a single column
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    TSlice this[int x, Range y] { get; }
    
    /// <summary>
    /// 2d slice indexer on a single row
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    TSlice this[Range x, int y] { get; }
    
    /// <summary>
    /// Get the given rows, this is the same as slicing with ".." for the columns
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    TSlice Rows(Interval<int> interval);

    /// <inheritdoc cref="Rows(AdventToolkit.New.Space.Bound.Interval{int})"/>
    TSlice Rows(Range range);

    /// <summary>
    /// Get the given columns, this is the same as slicing with ".." for the rows.
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    TSlice Cols(Interval<int> interval);

    /// <inheritdoc cref="Cols(AdventToolkit.New.Space.Bound.Interval{int})"/>
    TSlice Cols(Range range);
}