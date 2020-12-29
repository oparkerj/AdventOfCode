using System;
using System.Collections.Generic;
using System.Threading;

namespace AdventOfCode2019
{
    public class Computer<T>
        where T : struct
    {
        // Data
        public readonly List<T> Program = new();
        private Dictionary<int, Action<Computer<T>>> _ops = new();
        
        // State
        private int _pointer = 0;

        public int Pointer
        {
            get => _pointer;
            set => Interlocked.Exchange(ref _pointer, value);
        }
    }
}