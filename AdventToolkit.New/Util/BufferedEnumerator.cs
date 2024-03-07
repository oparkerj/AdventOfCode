using System.Collections;

namespace AdventToolkit.New.Util;

/// <summary>
/// Wrapper for an enumerator that supports buffering values ahead of time.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class BufferedEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> _source;

    private readonly Queue<T> _buffer;

    private int _bufferStop;

    /// <summary>
    /// Wrap an enumerator.
    /// </summary>
    /// <param name="source"></param>
    public BufferedEnumerator(IEnumerator<T> source)
    {
        _source = source;
        _buffer = new Queue<T>();
    }
    
    /// <summary>
    /// Create a buffer for a sequence.
    /// </summary>
    /// <param name="source"></param>
    public BufferedEnumerator(IEnumerable<T> source) : this(source.GetEnumerator()) { }

    /// <summary>
    /// Wrap an enumerator with a given initial buffer capacity.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="capacity"></param>
    public BufferedEnumerator(IEnumerator<T> source, int capacity)
    {
        _source = source;
        _buffer = new Queue<T>(capacity);
    }

    /// <summary>
    /// Create a buffer for a sequence with a given initial buffer capacity.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="capacity"></param>
    public BufferedEnumerator(IEnumerable<T> source, int capacity) : this(source.GetEnumerator(), capacity) { }

    /// <summary>
    /// Try to fill the buffer to the given size.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    private bool TryFillBuffer(int amount)
    {
        while (_buffer.Count < amount)
        {
            if (!_source.MoveNext()) return false;
            _buffer.Enqueue(_source.Current);
        }
        return true;
    }

    /// <summary>
    /// Number of items currently available in the buffer.
    /// </summary>
    public int BufferedCount => Math.Max(_buffer.Count - _bufferStop, 0);

    /// <summary>
    /// Set the number of items that can't count towards the buffer size.
    /// This can ensure there are a certain number of items available after
    /// buffering methods.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool SetBufferStop(int amount)
    {
        _bufferStop = amount;
        return TryFillBuffer(amount);
    }

    /// <summary>
    /// Try to read values from the enumerator until the buffer reaches the specified size.
    /// </summary>
    /// <param name="amount">Number of values to buffer.</param>
    /// <returns>True if the buffer was filled to the requested size, false otherwise.</returns>
    public bool TryBuffer(int amount) => TryFillBuffer(amount + _bufferStop);

    /// <summary>
    /// Pull an item from the buffer.
    /// This assumes the buffer is not empty.
    /// </summary>
    /// <returns>Next item.</returns>
    public T Next() => Current = _buffer.Dequeue();

    public bool MoveNext()
    {
        if (_buffer.TryDequeue(out var next))
        {
            Current = next;
            return true;
        }
        if (!_source.MoveNext()) return false;
        Current = _source.Current;
        return true;
    }

    public void Reset()
    {
        _source.Reset();
        _buffer.Clear();
    }

    public void Dispose()
    {
        _source.Dispose();
        _buffer.Clear();
    }

    public T Current { get; private set; } = default!;

    object? IEnumerator.Current => Current;
}