using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace AdventOfCode2019.IntCode
{
    public class DataLink
    {
        private readonly BlockingCollection<long> _data = new();

        public Func<long> FallbackOutput;

        public DataLink(Func<long> fallbackOutput = null)
        {
            FallbackOutput = fallbackOutput;
        }

        public DataLink(Computer computer, Func<long> fallbackOutput = null) : this(fallbackOutput)
        {
            computer.LineIn = Output;
        }

        public int Count => _data.Count;

        public bool HasNext => _data.Count > 0;

        public void Link(Computer output, Computer input)
        {
            output.LineOut = Input;
            input.LineIn = Output;
        }

        public bool TryTake(out long data) => _data.TryTake(out data);

        public void Insert(long data) => _data.Add(data);

        public void InsertMany(IEnumerable<long> data) => data.ForEach(Insert);

        public void InsertAscii(string s) => InsertMany(s.Select(c => (long) c));

        public long Output()
        {
            if (_data.Count > 0 || FallbackOutput == null) return _data.Take();
            return FallbackOutput();
        }

        public void Input(long data) => _data.Add(data);
    }
}