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