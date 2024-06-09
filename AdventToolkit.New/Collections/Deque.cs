using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventToolkit.New.Collections.Util;

namespace AdventToolkit.New.Collections;

/// <summary>
/// Double-ended queue container.
/// This collection provides fast access to the front and back
/// of the sequence.
///
/// This class is implemented using an array with front and back pointers.
/// The internal array has a size one larger than the desired capacity, which
/// makes it trivial to tell the difference between an empty and full queue.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Deque<T> : IEnumerable<T>
{
    // If capacity is less than this, no items can be stored
    private const int MinimumCapacity = 2;

    /// <summary>
    /// Minimum capacity of the queue when growing.
    /// </summary>
    public const int DefaultCapacity = 4;
    
    private T[] _data;
    private int _front;
    private int _back;

    /// <summary>
    /// If true, then push operations will automatically increase
    /// the queue capacity when full.
    /// </summary>
    public bool GrowIfFull { get; set; }

    /// <summary>
    /// Create a deque with the given capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public Deque(int capacity)
    {
        _data = new T[capacity + 1];
    }

    /// <summary>
    /// Create an empty deque.
    /// This will set <see cref="GrowIfFull"/> to true.
    /// </summary>
    public Deque()
    {
        _data = [];
        GrowIfFull = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private int Next(int i) => i >= _data.Length ? 0 : i + 1;

    /// <summary>
    /// Get the index of the previous element in the queue.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private int Prev(int i) => i <= 0 ? _data.Length - 1 : i - 1;

    /// <summary>
    /// Get the value from the internal array, wrapping the index
    /// if necessary.
    /// The index is assumed to be in the range [0, _data.Length + Capacity)
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private T GetWrap(int i)
    {
        var target = i + _back;
        Debug.Assert(target >= 0 && target < _data.Length + Capacity);
        return target < _data.Length ? _data[target] : _data[target - _data.Length];
    }

    /// <summary>
    /// Resize the internal array to increase the capacity.
    /// By default, this will double the capacity. If the array is empty
    /// the capacity will be set to <see cref="DefaultCapacity"/>
    /// </summary>
    private void Grow()
    {
        var size = Math.Max(Capacity * 2, DefaultCapacity) + 1;
        if ((uint) size > Array.MaxLength)
        {
            size = Array.MaxLength;
        }
        // +2 because we want at least Capacity + 1 but the internal
        // array is always one larger than the desired capacity.
        if (size < Capacity + 2)
        {
            size = Capacity + 2;
        }
        
        var next = new T[size];
        if (Empty)
        {
            _data = next;
            _front = _back = 0;
        }
        else if (_front < _back)
        {
            _data.AsSpan(0, _front).CopyTo(next);
            _data.AsSpan(_back).CopyTo(next.AsSpan(_front));
            _front = _data.Length - _back + _front;
            _back = 0;
            _data = next;
        }
        else
        {
            _data.AsSpan(_back, _front - _back).CopyTo(next);
            _front -= _back;
            _back = 0;
            _data = next;
        }
    }

    /// <summary>
    /// Current capacity of the queue.
    /// This is the max number of items that can be stored at the queue's
    /// current size.
    /// </summary>
    public int Capacity => Math.Max(_data.Length - 1, 0);

    /// <summary>
    /// Get the number of items in the queue.
    /// </summary>
    public int Count => _front < _back ? _data.Length - _back + _front : _front - _back;

    /// <summary>
    /// Check if the queue is empty.
    /// </summary>
    public bool Empty => _front == _back;

    /// <summary>
    /// Check if the queue is full.
    /// </summary>
    public bool Full => Next(_front) == _back;

    /// <summary>
    /// Index the current items in the queue.
    /// </summary>
    /// <param name="i"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public T this[int i]
    {
        get
        {
            Debug.Assert(i >= 0 && i < Count);
            return GetWrap(i);
        }
    }

    /// <summary>
    /// Alias for <see cref="PushFront"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Push(T item) => PushFront(item);

    /// <summary>
    /// Alias for <see cref="ForcePushFront"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ForcePush(T item) => ForcePushFront(item);

    /// <summary>
    /// Alias for <see cref="PopFront"/>
    /// </summary>
    /// <returns></returns>
    public T Pop() => PopFront();

    /// <summary>
    /// Alias for <see cref="TryPopFront"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryPop(out T item) => TryPopFront(out item);

    /// <summary>
    /// Push an item on the front of the queue.
    /// If the queue is full and <see cref="GrowIfFull"/> if true,
    /// the capacity will be increase to add the item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool PushFront(T item)
    {
        if (Full)
        {
            if (GrowIfFull)
            {
                Grow();
            }
            else return false;
        }
        
        _data[_front] = item;
        _front = Next(_front);
        return true;
    }

    /// <summary>
    /// Similar to <see cref="PushFront"/>, but if the queue is full,
    /// an item will be popped from the back in order to add the item.
    /// I.e. this method overwrites the back of the queue instead
    /// of growing the queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ForcePushFront(T item)
    {
        if (Full)
        {
            if (_data.Length >= MinimumCapacity)
            {
                PopBack();
            }
            else return false;
        }
        
        _data[_front] = item;
        _front = Next(_front);
        return true;
    }

    /// <summary>
    /// Pop an item from the front of the queue.
    /// </summary>
    /// <returns></returns>
    public T PopFront()
    {
        Debug.Assert(!Empty);

        _front = Prev(_front);
        var result = _data[_front];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _data[_front] = default!;
        }
        return result;
    }

    /// <summary>
    /// Attempt to pop an item from the front of the queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryPopFront(out T item)
    {
        if (Empty)
        {
            item = default!;
            return false;
        }

        item = PopFront();
        return true;
    }

    /// <summary>
    /// Push an item on the back of the queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool PushBack(T item)
    {
        if (Full)
        {
            if (GrowIfFull)
            {
                Grow();
            }
            else return false;
        }
        
        _back = Prev(_back);
        _data[_back] = item;
        return true;
    }

    /// <summary>
    /// Similar to <see cref="PushBack"/>, but if the queue is full,
    /// an item will be popped from the front in order to add the item.
    /// I.e. this method overwrites the front of the queue instead
    /// of growing the queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ForcePushBack(T item)
    {
        if (Full)
        {
            if (_data.Length >= MinimumCapacity)
            {
                PopFront();
            }
            else return false;
        }
        
        _back = Prev(_back);
        _data[_back] = item;
        return true;
    }

    /// <summary>
    /// Pop an item from the back of the queue.
    /// </summary>
    /// <returns></returns>
    public T PopBack()
    {
        Debug.Assert(!Empty);
        
        var result = _data[_back];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _data[_back] = default!;
        }
        _back = Next(_back);
        return result;
    }

    /// <summary>
    /// Attempt to pop an item from the back of the queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryPopBack(out T item)
    {
        if (Empty)
        {
            item = default!;
            return false;
        }

        item = PopBack();
        return true;
    }

    public ArrayEnumerator<T> GetEnumerator() => new(_data, _back, Prev(_front));

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}