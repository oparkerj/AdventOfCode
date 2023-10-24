using System.Buffers;

namespace AdventToolkit.New.Data;

public readonly struct Arr<T> : IDisposable
{
    public readonly T[] Data;
    public readonly int Length;

    public Arr(int length)
    {
        Data = ArrayPool<T>.Shared.Rent(length);
        Length = length;
    }

    public static Arr<T> Get(int length) => new(length);

    public static Arr<T> Temp(ReadOnlySpan<T> span)
    {
        var arr = Get(span.Length);
        span.CopyTo(arr);
        return arr;
    }

    public void Dispose() => ArrayPool<T>.Shared.Return(Data);

    public void CopyTo(Span<T> destination) => ((Span<T>) this).CopyTo(destination);

    public static implicit operator Span<T>(Arr<T> arr) => arr.Data.AsSpan(0, arr.Length);
    
    public static implicit operator ReadOnlySpan<T>(Arr<T> arr) => arr.Data.AsSpan(0, arr.Length);

    public T this[int i]
    {
        get => Data[i];
        set => Data[i] = value;
    }
}