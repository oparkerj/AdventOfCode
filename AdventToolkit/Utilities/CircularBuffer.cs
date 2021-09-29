using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
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

        public int NextIndex => _index;

        public int Count => _size;

        public bool Full => _size == Data.Length;

        public void Add(T t)
        {
            Data[_index] = t;
            if (_size < Data.Length) _size++;
            if (++_index == Data.Length) _index = 0;
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