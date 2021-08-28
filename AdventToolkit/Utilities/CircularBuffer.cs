using System.Collections;
using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        public readonly T[] Data;
        private int _index;

        public CircularBuffer(int size)
        {
            Data = new T[size];
        }

        public void Add(T t)
        {
            Data[_index] = t;
            if (++_index == Data.Length) _index = 0;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
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
    }
}