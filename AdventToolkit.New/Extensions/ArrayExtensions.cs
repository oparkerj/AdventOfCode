using System.Diagnostics;

namespace AdventToolkit.New.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Insert an item into an array. The element at the index will be shifted to the right.
    /// This will cause the last item to be dropped from the array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index">Insertion index.</param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertRight<T>(this T[] array, int index, T item)
    {
        Debug.Assert(index >= 0 && index < array.Length);
        Array.Copy(array, index, array, index + 1, array.Length - index - 1);
        array[index] = item;
    }
    
    /// <summary>
    /// Insert an item into an array. The element at the index will be shifted to the left.
    /// This will cause the first item to be dropped from the array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index">Insertion index.</param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertLeft<T>(this T[] array, int index, T item)
    {
        Debug.Assert(index >= 0 && index < array.Length);
        Array.Copy(array, 1, array, 0, index);
        array[index] = item;
    }
}