using System.Diagnostics;
using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Algorithms;

public static class Seq
{
    /// <summary>
    /// Yield sequences of the given length from
    /// [lower, .., lower] to [upper, .., upper].
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="lower">Lower bound.</param>
    /// <param name="upper">Upper bound.</param>
    /// <param name="length">Sequence length.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T[]> Span<T>(T lower, T upper, int length)
        where T : INumber<T>
    {
        Debug.Assert(lower <= upper);
        
        var arr = new T[length];
        Array.Fill(arr, lower);

        while (true)
        {
            yield return arr;

            var index = 0;
            while (true)
            {
                // Increment column
                if (arr[index] < upper)
                {
                    ++arr[index];
                    break;
                }
                // Spill to next column
                arr[index] = lower;
                // If last column spills, we are done
                if (++index == length) yield break;
            }
        }
    }
    
    /// <summary>
    /// Yield sequences with the given intervals on each column.
    /// Yields from [limits[0].Start, .., limits[N - 1].Start]
    /// to [limits[0].Last, .., limits[N - 1].Last]
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="limits"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T[]> From<T>(params Interval<T>[] limits)
        where T : INumber<T>
    {
        Debug.Assert(limits.All(limit => limit.Length >= T.Zero));
        
        var arr = new T[limits.Length];
        for (var i = 0; i < limits.Length; ++i)
        {
            arr[i] = limits[i].Start;
        }

        while (true)
        {
            yield return arr;

            var index = 0;
            while (true)
            {
                // Increment column
                if (arr[index] < limits[index].Last)
                {
                    ++arr[index];
                    break;
                }
                // Spill to next column
                arr[index] = limits[index].Start;
                // If last column spills, we are done
                if (++index == limits.Length) yield break;
            }
        }
    }

    /// <summary>
    /// Yields sequences of strictly increasing values.
    /// The initial sequence is [start, start + 1, .., start + length - 1].
    /// The final sequence is the initial sequence increased by the delta.
    /// Generates every sequence between the initial and final sequence.
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="start">Sequence start.</param>
    /// <param name="delta">Amount to increase initial sequence to get final sequence.</param>
    /// <param name="length">Sequence length.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T[]> Increasing<T>(T start, T delta, int length)
        where T : INumber<T>
    {
        Debug.Assert(delta >= T.Zero);
        
        var arr = new T[length];
        var upper = new T[length];
    
        {
            var value = start;
            for (var i = 0; i < length; ++i)
            {
                arr[i] = value;
                upper[i] = value++ + delta;
            }
        }

        while (true)
        {
            yield return arr;
            
            // Find next number to be incremented
            var index = length - 1;
            while (arr[index] >= upper[index])
            {
                if (--index < 0) yield break;
            }

            // From the current value to the end of the array,
            // assign increasing values.
            var last = arr[index];
            do
            {
                arr[index++] = ++last;
            } while (index < length);
        }
    }
}