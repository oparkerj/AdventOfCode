using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019
{
    public class Computer
    {
        public readonly LazyExpandingArray<long> Program;
        private readonly Dictionary<int, Action> _ops = new();
        
        private int _pointer = 0;

        public Computer(long[] program)
        {
            Program = new LazyExpandingArray<long>(program);
            AddOpcodes();
        }

        public static Computer From(string input)
        {
            return new(input.Csv().Longs().ToArray());
        }

        private void AddOpcodes()
        {
            _ops[1] = Add;
            _ops[2] = Mul;
            _ops[99] = Halt;
            _ops.TrimExcess();
        }

        public int Pointer
        {
            get => _pointer;
            set => Interlocked.Exchange(ref _pointer, value);
        }

        public long this[int pos]
        {
            get => Program[pos];
            set => Program[pos] = value;
        }

        public void Execute()
        {
            while (Pointer >= 0 && Pointer < Program.Length)
            {
                var op = (int) Program[Pointer];
                _ops[op]();
            }
        }

        private void Advance(int args = 0) => Pointer += args + 1;

        private long Arg(int relative) => Program[Addr(relative)];

        private int Addr(int relative)
        {
            return (int) Program[Pointer + relative];
        }

        private void Add()
        {
            Program[Addr(3)] = Arg(1) + Arg(2);
            Advance(3);
        }

        private void Mul()
        {
            Program[Addr(3)] = Arg(1) * Arg(2);
            Advance(3);
        }

        private void Halt()
        {
            Pointer = Program.Length;
        }
    }
}