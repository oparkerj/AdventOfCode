using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    // Class behaves like an array up to an initial capacity.
    // When reading/writing past the initial capacity, the data is stored
    // sparsely. When requested, the array can be resized and the sparse
    // data will be written into the main array.
    public class LazyExpandingArray<T> : IEnumerable<T>
    {
        private T[] _content;
        private readonly Line<T> _extra = new();
        private int _max;

        public LazyExpandingArray(T[] arr)
        {
            _content = arr;
            _max = arr.Length - 1;
        }

        public LazyExpandingArray(int size)
        {
            _content = new T[size];
            _max = size - 1;
        }

        public int Length => _max + 1;

        public int FastLength => _content.Length;

        public T this[int index]
        {
            get => index < _content.Length ? _content[index] : _extra[index];
            set
            {
                if (index < _content.Length) _content[index] = value;
                else
                {
                    _extra[index] = value;
                    _max = Math.Max(_max, index);
                }
            }
        }

        public void Resize()
        {
            Array.Resize(ref _content, _max + 1);
            foreach (var (index, value) in _extra)
            {
                _content[index] = value;
            }
            _extra.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Enumerates the contents, which are only ordered up to FastLength
        public IEnumerator<T> GetEnumerator()
        {
            return _content.Concat(_extra.Values).GetEnumerator();
        }
    }
}