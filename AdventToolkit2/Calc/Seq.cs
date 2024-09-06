using System.Collections;
using System.Diagnostics;
using System.Numerics;
using AdventToolkit2.Debugging;
using AdventToolkit2.Space.Bound;

namespace AdventToolkit2.Calc;

public static class Seq
{
    /// <summary>
    /// This enumerator will increment the first value in the array,
    /// if the value exceeds the upper limit, it is reset to the lower limit and
    /// the process is repeated on the next index.
    /// The enumerator ends after the last index exceeds the upper limit.
    ///
    /// The array is assumed to have a length of at least 1, and initialized
    /// before the enumerator is used.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="lower"></param>
    /// <param name="upper"></param>
    /// <typeparam name="T"></typeparam>
    public struct SeqSingleRangeEnumerator<T>(T[] array, T lower, T upper) : IEnumerable<T[]>, IEnumerator<T[]>
        where T : INumber<T>
    {
        private bool _first = true;
        
        public T[] Current { get; } = array;

        object IEnumerator.Current => Current;
        
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }
            
            var i = 0;
            while (true)
            {
                // Increment column
                if (Current[i] < upper)
                {
                    ++Current[i];
                    return true;
                }
                // Spill to next column
                Current[i] = lower;
                // Once the last column spills, we are done
                if (++i == Current.Length) return false;
            }
        }
        
        public SeqSingleRangeEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator() => GetEnumerator();

        public void Reset() => Err.NotSupported();
        
        public void Dispose() { }
    }

    /// <summary>
    /// This enumerator behaves like <see cref="SeqSingleRangeEnumerator{T}"/> but each
    /// index can have a different range of values.
    ///
    /// The array is assumed to have a length of at least 1, and initialized
    /// before the enumerator is used.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="limits"></param>
    /// <typeparam name="T"></typeparam>
    public struct SeqMultiRangeEnumerator<T>(T[] array, Interval<T>[] limits) : IEnumerable<T[]>, IEnumerator<T[]>
        where T : INumber<T>
    {
        private bool _first = true;

        public T[] Current { get; } = array;
        
        object IEnumerator.Current => Current;
        
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }
            
            var i = 0;
            while (true)
            {
                // Increment column
                if (Current[i] < limits[i].Last)
                {
                    ++Current[i];
                    return true;
                }
                // Spill to next column
                Current[i] = limits[i].Start;
                // Once the last column spills, we are done
                if (++i == Current.Length) return false;
            }
        }

        public SeqMultiRangeEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator() => GetEnumerator();

        public void Reset() => Err.NotSupported();

        public void Dispose() { }
    }

    /// <summary>
    /// This enumerator will search backwards through the array until it finds
    /// a value that is less than the corresponding upper value. This value will be
    /// incremented and increasing values will be assigned all the way to the end of
    /// the array.
    ///
    /// The array is assumed to have a length of at least 1, and initialized
    /// before the enumerator is used. The array and upper arrays are assumed
    /// to be the same length.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="upper"></param>
    /// <typeparam name="T"></typeparam>
    public struct SeqIncreasingEnumerator<T>(T[] array, T[] upper) : IEnumerable<T[]>, IEnumerator<T[]>
        where T : INumber<T>
    {
        private bool _first = true;

        public T[] Current { get; } = array;
        
        object IEnumerator.Current => Current;
        
        public bool MoveNext()
        {
            if (_first)
            {
                _first = false;
                return true;
            }
            
            // Find next number to be incremented
            var i = Current.Length - 1;
            while (Current[i] >= upper[i])
            {
                if (--i < 0) return false;
            }

            // From the current value to the end of the array, assign increasing values.
            var last = Current[i];
            do
            {
                Current[i++] = ++last;
            } while (i < Current.Length);
            return true;
        }

        public SeqIncreasingEnumerator<T> GetEnumerator() => this;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T[]> IEnumerable<T[]>.GetEnumerator() => GetEnumerator();

        public void Reset() => Err.NotSupported();

        public void Dispose() { }
    }

    /// <summary>
    /// Yield sequences from
    /// [lower, .., lower] to [upper, .., upper].
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="array">Buffer</param>
    /// <param name="lower">Lower bound (inclusive).</param>
    /// <param name="upper">Upper bound (inclusive).</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static SeqSingleRangeEnumerator<T> Span<T>(T[] array, T lower, T upper)
        where T : INumber<T>
    {
        Debug.Assert(array.Length > 0);
        Debug.Assert(lower <= upper);

        array.AsSpan().Fill(lower);
        return new SeqSingleRangeEnumerator<T>(array, lower, upper);
    }

    /// <inheritdoc cref="Span{T}(T[],T,T)"/>
    public static SeqSingleRangeEnumerator<T> Span<T>(int length, T lower, T upper)
        where T : INumber<T>
    {
        Debug.Assert(length > 0);
        return Span(new T[length], lower, upper);
    }

    /// <summary>
    /// Yield sequences with the given intervals on each column.
    /// Yields from [limits[0].Start, .., limits[N - 1].Start]
    /// to [limits[0].Last, .., limits[N - 1].Last]
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="limits"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static SeqMultiRangeEnumerator<T> From<T>(T[] array, params Interval<T>[] limits)
        where T : INumber<T>
    {
        Debug.Assert(limits.Length > 0);
        Debug.Assert(array.Length == limits.Length);
        Debug.Assert(limits.All(limit => limit.Length > T.Zero));
        
        for (var i = 0; i < limits.Length; ++i)
        {
            array[i] = limits[i].Start;
        }
        return new SeqMultiRangeEnumerator<T>(array, limits);
    }

    /// <inheritdoc cref="From{T}(T[],AdventToolkit2.Space.Bound.Interval{T}[])"/>
    public static SeqMultiRangeEnumerator<T> From<T>(params Interval<T>[] limits)
        where T : INumber<T>
    {
        Debug.Assert(limits.Length > 0);
        return From(new T[limits.Length], limits);
    }

    /// <summary>
    /// Yields sequences of strictly increasing values.
    /// The initial sequence is [start, start + 1, .., start + length - 1].
    /// The final sequence is the initial sequence increased by the delta.
    /// Generates every sequence between the initial and final sequence.
    /// The same array is yielded each time.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="upper"></param>
    /// <param name="start">Sequence start.</param>
    /// <param name="delta">Amount to increase initial sequence to get final sequence.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static SeqIncreasingEnumerator<T> Increasing<T>(T[] array, T[] upper, T start, T delta)
        where T : INumber<T>
    {
        Debug.Assert(delta >= T.Zero);
        Debug.Assert(array.Length > 0);
        Debug.Assert(array.Length == upper.Length);
    
        {
            var value = start;
            for (var i = 0; i < array.Length; ++i)
            {
                array[i] = value;
                upper[i] = (value++) + delta;
            }
        }

        return new SeqIncreasingEnumerator<T>(array, upper);
    }

    /// <inheritdoc cref="Increasing{T}(T[],T[],T,T)"/>
    public static SeqIncreasingEnumerator<T> Increasing<T>(T start, int length, T delta)
        where T : INumber<T>
    {
        Debug.Assert(length > 0);
        return Increasing(new T[length], new T[length], start, delta);
    }
}