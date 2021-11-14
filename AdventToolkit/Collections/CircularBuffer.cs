using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        public readonly T[] Data;
        private int _index;
        private int _size;

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

        public T this[int index]
        {
            get => Data[index.CircularMod(Data.Length)];
            set
            {
                var realIndex = index.CircularMod(Data.Length);
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

        public void RotateTo(int offset)
        {
            _index = offset.CircularMod(_size);
            var data = Data.ToArray(Data.Length);
            Array.Copy(data, _index, Data, 0, data.Length - _index);
            if (_index > 0) Array.Copy(data, 0, Data, data.Length - _index, _index);
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
    }
}