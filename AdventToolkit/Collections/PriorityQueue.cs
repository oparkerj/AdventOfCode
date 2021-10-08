using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventToolkit.Collections
{
    // Simple implementation of priority queue based off the .NET 6 implementation.
    public class PriorityQueue<T, TPriority> : IEnumerable<(T Value, TPriority Priority)>
    {
        private const int ElementChildren = 2;
        
        private (T Value, TPriority Priority)[] _elements;

        private int _count;

        private IComparer<TPriority> _comparer;

        public PriorityQueue()
        {
            _elements = Array.Empty<(T, TPriority)>();
            _comparer = Comparer<TPriority>.Default;
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            _elements = Array.Empty<(T, TPriority)>();
            _comparer = comparer ?? Comparer<TPriority>.Default;
        }

        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer = null)
        {
            _elements = new (T Value, TPriority Priority)[initialCapacity];
            _comparer = comparer ?? Comparer<TPriority>.Default;
        }

        public PriorityQueue(IEnumerable<(T Value, TPriority priority)> elements, IComparer<TPriority> comparer = null)
        {
            _elements = elements.ToArray();
            _comparer = comparer ?? Comparer<TPriority>.Default;
            _count = _elements.Length;
            if (_count > 1) Heapify();
        }

        public int Count => _count;

        private int ParentIndex(int index) => (index - 1) / ElementChildren;

        private int ChildrenIndex(int index) => index * ElementChildren + 1;

        private void Heapify()
        {
            var elements = _elements;
            var start = ParentIndex(_count - 1);
            for (var i = start; i >= 0; i--)
            {
                Sink(elements[i], i);
            }
        }

        private void Resize(int min)
        {
            var size = _elements.Length * 2;
            size = Math.Max(size, _elements.Length + 4);
            Array.Resize(ref _elements, size);
        }

        private void Bubble((T Value, TPriority Priority) element, int index)
        {
            var elements = _elements;
            while (index > 0)
            {
                var parentIndex = ParentIndex(index);
                var parent = elements[parentIndex];
                if (_comparer.Compare(element.Priority, parent.Priority) < 0)
                {
                    elements[index] = parent;
                    index = parentIndex;
                }
                else break;
            }
            elements[index] = element;
        }

        private void Sink((T Value, TPriority Priority) element, int index)
        {
            var elements = _elements;
            while (ChildrenIndex(index) is var i && i < _count)
            {
                var min = elements[i];
                var minIndex = i;
                var bound = Math.Min(i + ElementChildren, _count);
                while (++i < bound)
                {
                    var next = elements[i];
                    if (_comparer.Compare(next.Priority, min.Priority) < 0)
                    {
                        min = next;
                        minIndex = i;
                    }
                }
                if (_comparer.Compare(element.Priority, min.Priority) <= 0)
                {
                    break;
                }
                elements[index] = min;
                index = minIndex;
            }
            elements[index] = element;
        }

        private void RemoveFirst()
        {
            var lastIndex = --_count;
            if (lastIndex > 0)
            {
                var last = _elements[lastIndex];
                Sink(last, 0);
            }
            if (RuntimeHelpers.IsReferenceOrContainsReferences<(T, TPriority)>())
            {
                _elements[lastIndex] = default;
            }
        }

        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<(T, TPriority)>())
            {
                Array.Clear(_elements, 0, _count);
            }
            _count = 0;
        }

        public void Enqueue(T value, TPriority priority)
        {
            if (_count == _elements.Length)
            {
                Resize(_count + 1);
            }
            _count++;
            Bubble((value, priority), _count - 1);
        }

        public T Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("Queue is empty.");
            var value = _elements[0].Value;
            RemoveFirst();
            return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<(T Value, TPriority Priority)> GetEnumerator() => _elements.Take(_count).GetEnumerator();
    }

    public class PriorityQueue<T> : PriorityQueue<T, int>
    {
        public PriorityQueue() { }
        public PriorityQueue(IComparer<int> comparer) : base(comparer) { }
        public PriorityQueue(int initialCapacity, IComparer<int> comparer = null) : base(initialCapacity, comparer) { }
        public PriorityQueue(IEnumerable<(T Value, int priority)> elements, IComparer<int> comparer = null) : base(elements, comparer) { }
    }

    public class SelfPriorityQueue<T> : PriorityQueue<T, T>
    {
        public SelfPriorityQueue() { }
        public SelfPriorityQueue(IComparer<T> comparer) : base(comparer) { }
        public SelfPriorityQueue(int initialCapacity, IComparer<T> comparer = null) : base(initialCapacity, comparer) { }
        public SelfPriorityQueue(IEnumerable<(T Value, T priority)> elements, IComparer<T> comparer = null) : base(elements, comparer) { }

        public void Enqueue(T value) => Enqueue(value, value);
    }
}