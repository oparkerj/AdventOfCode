using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2019
{
    public class Computer
    {
        public readonly LazyExpandingArray<long> Program;
        private readonly Dictionary<int, Action> _ops = new();

        public Func<long> LineIn;
        public Action<long> LineOut;
        
        private int _pointer = 0;
        private int _interrupt = 0;
        private int _relativeBase = 0;

        public Computer(long[] program)
        {
            Program = new LazyExpandingArray<long>(program);
            AddOpcodes();
        }

        public static long[] Parse(string input) => input.Csv().Longs().ToArray();

        public static Computer From(string input) => new(Parse(input));

        public static Func<long> ConsoleReader()
        {
            return () => long.Parse(Console.ReadLine() ?? "0");
        }

        public static Action<long> ConsoleOutput()
        {
            return data => Console.WriteLine(data);
        }

        private void AddOpcodes()
        {
            _ops[1] = Add;
            _ops[2] = Mul;
            _ops[3] = Input;
            _ops[4] = Output;
            _ops[5] = JumpIfTrue;
            _ops[6] = JumpIfFalse;
            _ops[7] = LessThan;
            _ops[8] = Equals;
            _ops[9] = SetRelativeBase;
            _ops[99] = Halt;
            _ops.TrimExcess();
        }

        public int Pointer
        {
            get => _pointer;
            set => Interlocked.Exchange(ref _pointer, value);
        }

        public bool Interrupt
        {
            get => _interrupt != 0;
            set => Interlocked.Exchange(ref _interrupt, value ? 1 : 0);
        }

        public int RelativeBase
        {
            get => _relativeBase;
            set => Interlocked.Exchange(ref _relativeBase, value);
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
                op %= 100;
                _ops[op]();
                if (!Interrupt) continue;
                Interrupt = false;
                break;
            }
        }

        public Task ExecuteAsync()
        {
            return Task.Run(Execute);
        }

        public long NextOutput()
        {
            var oldOutput = LineOut;
            var data = new DataLink();
            LineOut = data.Input;
            Execute();
            LineOut = oldOutput;
            data.TryTake(out var result);
            return result;
        }

        private void Advance(int args = 0) => Pointer += args + 1;

        private int ParameterMode(int relative) => (int) Program[Pointer] / 10.Pow(relative + 1) % 10;

        private long Arg(int relative)
        {
            var mode = ParameterMode(relative);
            if (mode is 0 or 2) return Program[Addr(relative)];
            return Program[Pointer + relative];
        }

        private int Addr(int relative, int mode)
        {
            if (mode == 0) return (int) Program[Pointer + relative];
            return (int) Program[Pointer + relative] + RelativeBase;
        }

        private int Addr(int relative)
        {
            return Addr(relative, ParameterMode(relative));
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

        private void Input()
        {
            Program[Addr(1)] = LineIn();
            Advance(1);
        }

        private void Output()
        {
            LineOut(Arg(1));
            Advance(1);
        }

        private void JumpIfTrue()
        {
            if (Arg(1) != 0) Pointer = (int) Arg(2);
            else Advance(2);
        }

        private void JumpIfFalse()
        {
            if (Arg(1) == 0) Pointer = (int) Arg(2);
            else Advance(2);
        }

        private void LessThan()
        {
            var write = Arg(1) < Arg(2) ? 1 : 0;
            Program[Addr(3)] = write;
            Advance(3);
        }

        private void Equals()
        {
            var write = Arg(1) == Arg(2) ? 1 : 0;
            Program[Addr(3)] = write;
            Advance(3);
        }

        private void SetRelativeBase()
        {
            RelativeBase += (int) Arg(1);
            Advance(1);
        }

        private void Halt()
        {
            Pointer = Program.Length;
        }
    }
}