using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.IntCode;

public class ComputerInput : IDisposable
{
    private IEnumerator<long> _source;

    public ComputerInput(IEnumerable<long> source)
    {
        _source = source.GetEnumerator();
    }

    public ComputerInput(IEnumerable<int> source)
    {
        _source = source.Select(i => (long) i).GetEnumerator();
    }

    public ComputerInput(IEnumerable<bool> source)
    {
        _source = source.Select(b => b ? 1L : 0L).GetEnumerator();
    }

    public ComputerInput(params long[] input) : this((IEnumerable<long>) input) { }

    public void Dispose()
    {
        if (_source == null) return;
        _source.Dispose();
        _source = null;
    }

    public long Line()
    {
        if (_source == null) throw new Exception("Input is exhausted.");
        if (!_source.MoveNext()) throw new Exception("Reached end of source.");
        return _source.Current;
    }
}