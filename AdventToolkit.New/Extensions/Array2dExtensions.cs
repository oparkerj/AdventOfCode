namespace AdventToolkit.New.Extensions;

public static class Array2dExtensions
{
    /// <summary>
    /// Get an enumerator for the values of a 2D-array.
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Enumerate<T>(this T[,] array)
    {
        var width = array.GetLength(0);
        var height = array.GetLength(1);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                yield return array[x, y];
            }
        }
    }

    /// <summary>
    /// Check if a 2D array contains a value.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool Contains<T>(this T[,] array, T value)
    {
        var width = array.GetLength(0);
        var height = array.GetLength(1);
        
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (Equals(array[x, y], value)) return true;
            }
        }

        return false;
    }
}