using System;
using System.Collections.Generic;

namespace AdventToolkit.Utilities;

// Allows access to elements of an enumerable through successive calls to Next().
// This should be used when the length of the input is known.
public readonly struct Iter<T>(IEnumerator<T> source) : IDisposable
{
    public Iter(IEnumerable<T> source) : this(source.GetEnumerator()) { }

    public T Next()
    {
        if (!source.MoveNext()) throw new ArgumentOutOfRangeException(nameof(source), "Reached end of sequence");
        return source.Current;
    }
        
    public void Dispose() => source.Dispose();
}