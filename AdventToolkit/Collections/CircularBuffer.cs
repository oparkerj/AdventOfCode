using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections;

public class CircularBuffer<T> : IEnumerable<T>
{
    public readonly T[] Data;
    private int _index;
    private int _size;
    
    // Offset is what is currently considered index 0
    public int Offset { get; set; }

    public CircularBuffer(int size)
    {
        Data = new T[size];
    }

    public CircularBuffer(T[] arr, bool isFilled = true)
    {
        Data = arr;
        _size = isFilled ? arr.Length : 0;
    }

    public int NextIndex => _index;

    public int Count => _size;

    public bool Full => _size == Data.Length;

    private int InternalIndex(int i) => (i + Offset).CircularMod(Data.Length);

    public T this[int index]
    {
        get => Data[InternalIndex(index)];
        set
        {
            var realIndex = InternalIndex(index);
            Data[realIndex] = value;
            _size = Math.Max(_size, realIndex + 1);
        }
    }

    public void Add(T t)
    {
        Data[_index] = t;
        if (_size < Data.Length) _size++;
        if (++_index == Data.Length) _index = 0;
    }

    public int IndexOf(T value)
    {
        for (var i = 0; i < _size; i++)
        {
            if (Equals(Data[i], value)) return i;
        }
        return -1;
    }

    public int RotatedIndexOf(T value)
    {
        var index = IndexOf(value);
        return index < 0 ? index : InternalIndex(index);
    }

    public static void RotateTo(T[] array, int offset)
    {
        offset = offset.CircularMod(array.Length);
        if (offset == 0) return;
        var data = array.ToArray(array.Length);
        Array.Copy(data, offset, array, 0, array.Length - offset);
        if (offset > 0) Array.Copy(data, 0, array, array.Length - offset, offset);
    }

    public int Find(Func<T, bool> predicate)
    {
        return (Array.FindIndex(Data, obj => predicate(obj)) - Offset).CircularMod(Data.Length);
    }

    // TODO fix?
    public void Move(long fromIndex, long toIndex)
    {
        Move((int) (fromIndex + Offset).CircularMod(Data.Length),
             (int) (toIndex + Offset).CircularMod(Data.Length));
    }

    // TODO fix?
    public void Move(int fromIndex, int toIndex)
    {
        var from = InternalIndex(fromIndex);
        var to = InternalIndex(toIndex);
        if (from == to) return;
        
        var t = Data[from];
        if (from < to)
        {
            Array.Copy(Data, from + 1, Data, from, to - from);
            if (Offset > from && Offset <= to) Offset--;
        }
        else
        {
            Array.Copy(Data, to, Data, to + 1, from - to);
            if (Offset >= to && Offset < from) Offset++;
        }
        Data[to] = t;
    }

    public void RotateTo(int offset)
    {
        RotateTo(Data, offset);
        _index = 0;
    }

    private IEnumerable<T> FullEnumerator()
    {
        var data = Data;
        for (var i = _index; i < data.Length; i++)
        {
            yield return data[i];
        }
        if (_index <= 0) yield break;
        for (var i = 0; i < _index; i++)
        {
            yield return data[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        if (_size == Data.Length) return FullEnumerator().GetEnumerator();
        return FullEnumerator().Take(_size).GetEnumerator();
    }

    public IEnumerable<T> Unordered() => _size == Data.Length ? Data : Data.Take(_size);
}