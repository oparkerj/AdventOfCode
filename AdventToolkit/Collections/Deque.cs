using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections;

// Double-ended queue backed by an array
public class Deque<T> : ICollection<T>
{
    private T[] _data;
    private int _tail, _head;
    private bool _empty = true;

    private const int DefaultSize = 4;

    public Deque(int capacity)
    {
        _data = new T[capacity];
    }
    
    public Deque() : this(DefaultSize) { }

    public Deque(IEnumerable<T> items)
    {
        if (items is ICollection<T> collection)
        {
            _data = new T[collection.Count];
            collection.CopyTo(_data, 0);
            _empty = collection.Count == 0;
        }
        else
        {
            _data = new T[DefaultSize];
            foreach (var item in items)
            {
                AddLast(item);
            }
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (_empty) yield break;

        var data = _data;
        var head = _head;
        var tail = _tail;

        var i = head;
        if (tail <= head)
        {
            while (i < data.Length) yield return data[i++];
        }
        i = 0;
        while (i < tail)
        {
            yield return data[i++];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public int Count => Full ? _data.Length : _tail < _head ? _tail - _head + _data.Length : _tail - _head;

    public bool Full => _tail == _head && !_empty;
    
    public bool IsReadOnly => false;

    private void ExpandIfFull()
    {
        if (!Full) return;
        if (_data.Length == int.MaxValue) throw new Exception("Deque cannot expand further.");
        var newSize = _data.Length * 2;
        if (newSize < 0)
        {
            newSize = int.MaxValue;
        }
        
        var dest = new T[newSize];
        if (_tail <= _head)
        {
            Array.Copy(_data, _head, dest, 0, _data.Length - _head);
            Array.Copy(_data, 0, dest, _data.Length - _head, _tail);
        }
        else Array.Copy(_data, _head, dest, 0, _tail - _head);

        _head = 0;
        _tail = _data.Length; // Old length
        _data = dest;
    }

    private void Decrement(ref int i)
    {
        i = (i - 1).CircularMod(_data.Length);
    }

    private void Increment(ref int i)
    {
        i = (i + 1).CircularMod(_data.Length);
    }

    public T First => _data[_head];

    public bool TryPeekFirst(out T first)
    {
        first = First;
        return !_empty;
    }

    public T Last => _data[_tail == 0 ? ^1 : _tail - 1];

    public bool TryPeekLast(out T last)
    {
        last = Last;
        return !_empty;
    }

    public void AddFirst(T item)
    {
        ExpandIfFull();
        Decrement(ref _head);
        _data[_head] = item;
        if (_empty) _empty = false;
    }

    public void AddLast(T item)
    {
        ExpandIfFull();
        _data[_tail] = item;
        Increment(ref _tail);
        if (_empty) _empty = false;
    }

    public void Add(T item) => AddLast(item);

    private void ResetEmpty()
    {
        _empty = _head == _tail;
        if (_empty) _head = _tail = 0;
    }

    private void InternalRemove(int index)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _data[index] = default;
        }
    }

    private T InternalRemoveFirst()
    {
        var result = _data[_head];
        InternalRemove(_head);
        Increment(ref _head);
        ResetEmpty();
        return result;
    }

    public T RemoveFirst()
    {
        if (_empty) throw new Exception("Deque is empty");
        return InternalRemoveFirst();
    }

    public bool TryRemoveFirst(out T first)
    {
        if (_empty)
        {
            first = default;
            return false;
        }
        first = InternalRemoveFirst();
        return true;
    }

    private T InternalRemoveLast()
    {
        Decrement(ref _tail);
        var result = _data[_tail];
        InternalRemove(_tail);
        ResetEmpty();
        return result;
    }

    public T RemoveLast()
    {
        if (_empty) throw new Exception("Deque is empty");
        return InternalRemoveLast();
    }

    public bool TryRemoveLast(out T last)
    {
        if (_empty)
        {
            last = default;
            return false;
        }
        last = InternalRemoveLast();
        return true;
    }

    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Fill(_data, default);
        }
        _head = _tail = 0;
        _empty = true;
    }

    private int InternalIndex(T item)
    {
        if (_empty) return -1;
        
        if (_tail <= _head)
        {
            var index = Array.IndexOf(_data, item, _head, _data.Length - _head);
            return index > -1 ? index : Array.IndexOf(_data, item, 0, _tail);
        }
        return Array.IndexOf(_data, item, _head, _tail - _head);
    }

    public bool Contains(T item)
    {
        return InternalIndex(item) > -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex + Count >= array.Length) throw new ArgumentException("Not enough space.");
        
        var span = _data.AsSpan();
        var dest = array.AsSpan(arrayIndex);
        
        if (_tail <= _head)
        {
            span[_head..].CopyTo(dest);
            span[.._tail].CopyTo(dest[(span.Length - _head)..]);
        }
        else span[_head.._tail].CopyTo(dest);
    }

    public bool Remove(T item)
    {
        throw new NotSupportedException();
    }
}