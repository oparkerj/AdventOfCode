using System.Collections;
using AdventToolkit.New.Algorithms;
using AdventToolkit.New.Extensions;

namespace AdventToolkit.New.Data;

/// <summary>
/// An array wrapper where elements are added in a circular manner.
/// </summary>
/// <typeparam name="T">Array Type</typeparam>
public class CircularArray<T> : IEnumerable<T>
{
    public readonly T[] Data;
    
    /// <summary>
    /// Index in the array where the next element will be added.
    /// </summary>
    public int Pointer { get; set; }
    
    /// <summary>
    /// Size of the array.
    /// </summary>
    public int Length => Data.Length;

    /// <summary>
    /// Wrap an existing array.
    /// </summary>
    /// <param name="data">Input array.</param>
    public CircularArray(T[] data) => Data = data;

    /// <summary>
    /// Create a new array with a given length.
    /// </summary>
    /// <param name="length">Array length.</param>
    public CircularArray(int length) : this(new T[length]) { }

    /// <summary>
    /// Split the internal array into two parts at given index.
    /// The two parts can be concatenated to obtain the rotated array.
    /// </summary>
    /// <param name="index">Index of the start of the rotated array.</param>
    /// <returns>Rotated array split.</returns>
    private Split<T> GetSplit(int index) => Split<T>.FromReverse(Data, index);
    
    /// <summary>
    /// Add an item to the array.
    /// The item is inserted at the current pointer, which is
    /// then incremented to the next index, wrapping around
    /// if necessary.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        Data[Pointer] = item;
        if (++Pointer >= Data.Length)
        {
            Pointer = 0;
        }
    }
    
    /// <summary>
    /// Rotate the internal array to the given offset.
    /// The index given by offset will become the first element
    /// in the array. If no offset is given, the <see cref="Offset"/>
    /// property is used.
    /// </summary>
    /// <param name="offset">Rotate offset.</param>
    public void Align(int offset)
    {
        var target = offset.Mod(Data.Length);
        var (first, second) = GetSplit(target);
        if (first.Length < second.Length)
        {
            // var buffer = first.ToArray();
            using var buffer = Arr<T>.Temp(first);
            second.CopyTo(Data.AsSpan(first.Length));
            buffer.CopyTo(Data.AsSpan());
        }
        else
        {
            // var buffer = second.ToArray();
            using var buffer = Arr<T>.Temp(second);
            first.CopyTo(Data.AsSpan());
            buffer.CopyTo(Data.AsSpan(first.Length));
        }
    }

    /// <summary>
    /// Access an element at a rotated index.
    /// </summary>
    /// <param name="i">Rotated index.</param>
    public T this[int i]
    {
        get => Data[(i + Pointer).Mod(Data.Length)];
        set => Data[(i + Pointer).Mod(Data.Length)] = value;
    }

    /// <summary>
    /// Clear the internal array.
    /// Also resets <see cref="Offset"/> and <see cref="Pointer"/>.
    /// </summary>
    public void Clear()
    {
        Array.Clear(Data);
        Pointer = 0;
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        return Pointer == 0 ? Data.Enumerate() : Enumerate(Pointer);

        IEnumerator<T> Enumerate(int start)
        {
            if (Data.Length == 0) yield break;
            var i = start;
            do
            {
                yield return Data[i];
                if (++i >= Data.Length)
                {
                    i = 0;
                }
            } while (i != start);
        }
    }
}