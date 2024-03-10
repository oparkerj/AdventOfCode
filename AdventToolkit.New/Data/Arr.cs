using System.Buffers;

namespace AdventToolkit.New.Data;

/// <summary>
/// Represents a shared array.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Arr<T> : IDisposable
{
    /// <summary>
    /// Underlying array.
    /// </summary>
    public readonly T[] Data;
    
    /// <summary>
    /// Requested array length (Actual array may be longer).
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// Create an array with the given minimum length.
    /// </summary>
    /// <param name="length"></param>
    public Arr(int length)
    {
        Data = ArrayPool<T>.Shared.Rent(length);
        Length = length;
    }

    /// <summary>
    /// Get an array with the given minimum length.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Arr<T> Get(int length) => new(length);

    /// <summary>
    /// Create an array and copy the given span to it.
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public static Arr<T> Temp(ReadOnlySpan<T> span)
    {
        var arr = Get(span.Length);
        span.CopyTo(arr);
        return arr;
    }

    /// <summary>
    /// Returns the underlying array to the shared array pool.
    /// </summary>
    public void Dispose() => ArrayPool<T>.Shared.Return(Data);

    /// <summary>
    /// Copy the array to the destination span.
    /// </summary>
    /// <param name="destination"></param>
    public void CopyTo(Span<T> destination) => ((Span<T>) this).CopyTo(destination);

    /// <summary>
    /// Implicit conversion to span.
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static implicit operator Span<T>(Arr<T> arr) => arr.Data.AsSpan(0, arr.Length);
    
    /// <summary>
    /// Implicit conversion to read-only span.
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static implicit operator ReadOnlySpan<T>(Arr<T> arr) => arr.Data.AsSpan(0, arr.Length);

    /// <summary>
    /// Index the underlying array.
    /// </summary>
    /// <param name="i"></param>
    public T this[int i]
    {
        get => Data[i];
        set => Data[i] = value;
    }

    /// <summary>
    /// Get this array as a span.
    /// </summary>
    public Span<T> Span => this;
}