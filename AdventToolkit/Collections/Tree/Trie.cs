using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Collections.Tree;

public class Trie<TBin, T> : IEnumerable<TBin>
    where TBin : IEnumerable<T>
{
    public TrieNode<TBin, T> Root { get; private set; } = new(default);
    
    public int Count { get; private set; }

    public bool TryTrace(TrieNode<TBin, T> start, TBin value, out TrieNode<TBin, T> result)
    {
        var fail = value.Any(t => !start.TryGetNext(t, out start));
        result = start;
        return !fail;
    }

    private void AddInternal(TBin value, IEnumerable<T> sequence)
    {
        var last = sequence.Aggregate(Root, (node, t) => node.Add(t));
        if (!last.IsEnd)
        {
            last.Source = value;
            last.IsEnd = true;
            Count++;
        }
    }

    public void Add(TBin value) => AddInternal(value, value);

    public void AddReverse(TBin value) => AddInternal(value, value.Reverse());

    public bool Contains(TBin value)
    {
        return TryTrace(Root, value, out _);
    }
    
    // Find the smallest prefix present in the sequence.
    // This cannot find values which have another value as a prefix.
    public bool TryFindValue(IEnumerable<T> sequence, out TBin value)
    {
        if (Root.IsEnd)
        {
            value = Root.Source;
            return true;
        }
        
        var tracking = new List<TrieNode<TBin, T>>(2);

        foreach (var t in sequence)
        {
            for (var i = 0; i < tracking.Count; i++)
            {
                if (tracking[i].TryGetNext(t, out var next))
                {
                    if (next.IsEnd)
                    {
                        value = next.Source;
                        return true;
                    }
                    tracking[i] = next;
                }
                else
                {
                    tracking.RemoveConcurrent(ref i);
                }
            }
            if (Root.TryGetNext(t, out var branch))
            {
                if (branch.IsEnd)
                {
                    value = branch.Source;
                    return true;
                }
                tracking.Add(branch);
            }
        }

        value = default;
        return false;
    }

    public bool TryFindValueLast(IEnumerable<T> sequence, out TBin value)
    {
        return TryFindValue(sequence.Reverse(), out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TBin> GetEnumerator() => Root.GetEnumerator();
}

public class TrieNode<TBin, T> : IEnumerable<TBin>
{
    private Dictionary<T, TrieNode<TBin, T>> _children = new();
    
    public T Value { get; }
    
    public bool IsEnd { get; internal set; }
    
    public TBin Source { get; internal set; }

    public TrieNode(T value, bool isEnd = false)
    {
        Value = value;
        IsEnd = isEnd;
    }

    public bool TryGetNext(T t, out TrieNode<TBin, T> node) => _children.TryGetValue(t, out node);

    public TrieNode<TBin, T> Add(T t)
    {
        if (_children.TryGetValue(t, out var node)) return node;
        return _children[t] = new TrieNode<TBin, T>(t);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TBin> GetEnumerator()
    {
        if (IsEnd) yield return Source;
        foreach (var bin in _children.Values.Flatten())
        {
            yield return bin;
        }
    }
}