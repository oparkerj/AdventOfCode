namespace AdventToolkit.New.Collections.Interface;

/// <summary>
/// Interface to a 2d array.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IArray2d<T> : IEnumerable<T>
{
    /// <summary>
    /// Array width (length of first dimension)
    /// </summary>
    int Width { get; }
    
    /// <summary>
    /// Array height (length of second dimension)
    /// </summary>
    int Height { get; }
    
    /// <summary>
    /// Total size of the array.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Clear the array
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Array indexer
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    T this[int x, int y] { get; set; }

    /// <summary>
    /// Access a row of the array.
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    ArrayView<T> Row(int y);

    /// <summary>
    /// Access a column of the array.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    ArrayView<T> Col(int x);
}

