using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Threads;

namespace AdventOfCode2019.IntCode;

public class Computer : IIntCode
{
    public readonly LazyExpandingArray<long> Program;
    private readonly Dictionary<int, Action> _ops = new();
    private readonly DataLink _internalLink = new();

    public Func<long> Input { get; set; }
    public Action<long> Output { get; set; }
        
    private int _pointer = 0;
    private int _relativeBase = 0;

    public Computer(long[] program)
    {
        Program = new LazyExpandingArray<long>(program);
        AddOpcodes();
    }

    public Computer(string program) : this(Parse(program)) { }

    public static long[] Parse(string input) => input.Csv().Longs().ToArray();

    public static Computer From(string input) => new(Parse(input));

    public static Func<long> ConsoleReader()
    {
        return () => long.Parse(Console.ReadLine() ?? "0");
    }

    public static Func<long> AsciiReader()
    {
        return () =>
        {
            Read:
            var d = Console.Read();
            if (d == '\r') goto Read;
            return d;
        };
    }

    public static Action<long> ConsoleOutput()
    {
        return Console.WriteLine;
    }

    public static Action<long> AsciiOutput()
    {
        return data => Console.Write((char) data);
    }

    private void AddOpcodes()
    {
        _ops[1] = Add;
        _ops[2] = Mul;
        _ops[3] = ReadInput;
        _ops[4] = SendOutput;
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

    public Lock Interrupt { get; }

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

    public bool Ready => Pointer >= 0 && Pointer < Program.Length;

    public void Execute()
    {
        while (Ready)
        {
            var op = (int) Program[Pointer];
            op %= 100;
            _ops[op]();
            if (!Interrupt) continue;
            Interrupt.Toggle(false);
            break;
        }
    }

    public long NextOutput()
    {
        var oldOutput = Output;
        Output = _internalLink.Input;
        _ops[4] = OutputInterrupt;
        Execute();
        Output = oldOutput;
        _ops[4] = SendOutput;
        _internalLink.TryTake(out var result);
        return result;
    }

    public int NextInt() => (int) NextOutput();

    public bool NextBool() => NextOutput() != 0;

    public long LastOutput()
    {
        var oldOutput = Output;
        Output = _internalLink.Input;
        Execute();
        Output = oldOutput;
        while (_internalLink.Count > 1) _internalLink.TryTake(out _);
        _internalLink.TryTake(out var result);
        return result;
    }

    private void Advance(int args = 0) => Pointer += args + 1;

    private int ParameterMode(int relative) => (int) Program[Pointer] / 10.Pow(relative + 1) % 10;

    private long Arg(int relative)
    {
        var mode = ParameterMode(relative);
        if (mode is 0 or 2) return Program[Addr(relative, mode)];
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

    private void ReadInput()
    {
        Program[Addr(1)] = Input();
        Advance(1);
    }

    private void SendOutput()
    {
        Output(Arg(1));
        Advance(1);
    }

    private void OutputInterrupt()
    {
        Output(Arg(1));
        Advance(1);
        Interrupt.Toggle(true);
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