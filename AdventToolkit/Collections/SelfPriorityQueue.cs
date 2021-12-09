using System.Collections.Generic;

namespace AdventToolkit.Collections;

public class SelfPriorityQueue<TKey> : PriorityQueue<TKey, TKey>
{
    public SelfPriorityQueue() { }
    public SelfPriorityQueue(IComparer<TKey> comparer) : base(comparer) { }
    public SelfPriorityQueue(IEnumerable<(TKey Element, TKey Priority)> items) : base(items) { }
    public SelfPriorityQueue(IEnumerable<(TKey Element, TKey Priority)> items, IComparer<TKey> comparer) : base(items, comparer) { }
    public SelfPriorityQueue(int initialCapacity) : base(initialCapacity) { }
    public SelfPriorityQueue(int initialCapacity, IComparer<TKey> comparer) : base(initialCapacity, comparer) { }

    public void Enqueue(TKey key) => Enqueue(key, key);
}