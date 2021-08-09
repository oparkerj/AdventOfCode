using System;
using System.Collections.Concurrent;

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

        public void Link(Computer output, Computer input)
        {
            output.LineOut = Input;
            input.LineIn = Output;
        }

        public bool TryTake(out long data) => _data.TryTake(out data);

        public void Insert(long data) => _data.Add(data);

        public long Output()
        {
            if (_data.Count > 0 || FallbackOutput == null) return _data.Take();
            return FallbackOutput();
        }

        public void Input(long data) => _data.Add(data);
    }
}