using System;
using System.Collections.Generic;

namespace AdventOfCode2019.IntCode
{
    public class OutputSequence
    {
        public readonly List<Action<long>> Parts = new();
        private int _index = 0;

        public OutputSequence Then(Action<long> action)
        {
            Parts.Add(action);
            return this;
        }

        public OutputSequence ThenInt(Action<int> action)
        {
            return Then(data => action((int) data));
        }

        public OutputSequence ThenBool(Action<bool> action)
        {
            return Then(data => action(data != 0));
        }

        public OutputSequence ThenMultiple<TA>(int amount, Func<int, TA> create, Action<TA, int, long> write, Action<TA> consume)
        {
            var buf = create(amount);
            var current = this;
            for (var i = 0; i < amount; i++)
            {
                var i1 = i;
                current = current.Then(data => write(buf, i1, data));
            }
            return current.And(() => consume(buf));
        }

        public OutputSequence ThenMultiple(int amount, Action<long[]> action)
        {
            return ThenMultiple(amount, size => new long[size], (buf, i, data) => buf[i] = data, action);
        }
        
        public OutputSequence ThenMultipleInts(int amount, Action<int[]> action)
        {
            return ThenMultiple(amount, size => new int[size], (buf, i, data) => buf[i] = (int) data, action);
        }

        public OutputSequence And(Action action)
        {
            var wrap = Parts[^1];
            Parts[^1] = data =>
            {
                wrap(data);
                action();
            };
            return this;
        }

        public void Line(long data)
        {
            Parts[_index](data);
            _index++;
            if (_index >= Parts.Count) _index = 0;
        }
    }
}