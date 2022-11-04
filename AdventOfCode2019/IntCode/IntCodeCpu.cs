using System;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Computer;

namespace AdventOfCode2019.IntCode;

public class IntCodeCpu : Cpu<long>, IPipeline<long>, IInstructionSet<long>
{
    public int RelativeBase { get; set; }
    public (CpuFunc<long, OpArgs<long, int>, bool>, int)[] Actions;
    private OpArgs<long, int> _instruction;
    private bool _jumped;
    private int _argCount;

    public Func<long> Input;
    public Action<long> Output;

    public IntCodeCpu(long[] program)
    {
        base.Memory = new IntCodeMemory(program);
        Pipeline = this;
        InstructionSet = this;

        var builder = PrefixInstructionBuilder<long>.Default(long.Parse);
        builder.ClearBinders();
        builder.AddBinder("1", new IntCodeBind(this, 1));
        builder.AddBinder("2", new IntCodeBind(this, 2));
        builder.AddBinder("3", new IntCodeBind(this, 3));
        
        // Define the instructions
        builder.Add("halt", () => true);
        builder.Add("add 1 2 3", (a, b, c) => c.Value = a.Value + b.Value);
        builder.Add("mul 1 2 3", (a, b, c) => c.Value = a.Value * b.Value);
        builder.Add("input 1", a => a.Value = Input());
        builder.Add("output 1", a => Output(a.Value));
        builder.AddCpu("jumpIfTrue 1 2", (cpu, a, b) => a.When(Num.True, cpu.JumpTo, b));
        builder.AddCpu("jumpIfFalse 1 2", (cpu, a, b) => a.When(Num.False, cpu.JumpTo, b));
        builder.Add("lessThan 1 2 3", (a, b, c) => c.Value = (a.Value < b.Value).AsInt());
        builder.Add("equals 1 2 3", (a, b, c) => c.Value = (a.Value == b.Value).AsInt());
        builder.Add("setRelativeBase 1", a => RelativeBase += (int) a.Value);

        Actions = builder.Actions.Zip(builder.ArgCounts).ToArray();
        _instruction = builder.Parse(this, "add 0 0 0");
    }

    public IntCodeCpu(string program) : this(program.Csv().Longs().ToArray()) { }

    public static long[] Parse(string program) => program.Csv().Longs().ToArray();

    public new IntCodeMemory Memory
    {
        get => (IntCodeMemory) base.Memory;
        set => base.Memory = value;
    }

    public bool Tick(Cpu<long> cpu)
    {
        var halt = cpu.InstructionSet.ExecuteNext(cpu);
        if (!_jumped) cpu.Pointer += _argCount + 1;
        _jumped = false;
        return halt;
    }

    public void JumpRelative(Cpu<long> cpu, int offsetToNext)
    {
        cpu.Pointer += offsetToNext - 1;
        _jumped = true;
    }

    public void JumpTo(Cpu<long> cpu, int next)
    {
        cpu.Pointer = next - 1;
        _jumped = true;
    }

    public bool ExecuteNext(Cpu<long> cpu)
    {
        if (cpu.Pointer < 0 || cpu.Pointer >= Memory.Array.Length) return true;
        var (action, args) = Actions[(int) (Memory[Pointer] % 99)];
        _argCount = args;
        return action(cpu, _instruction);
    }
}

public class IntCodeMemory : IMemory<long>
{
    public LazyExpandingArray<long> Array;

    public IntCodeMemory(long[] program) => Array = new LazyExpandingArray<long>(program);

    public void Reset() => throw new System.NotImplementedException();

    public long this[long t]
    {
        get => this[(int) t];
        set => this[(int) t] = value;
    }

    public long this[int i]
    {
        get => Array[i];
        set => Array[i] = value;
    }

    public long this[char c]
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public long this[string s]
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }
}

public class IntCodeBind : Mem<long>
{
    public IntCodeBind(IntCodeCpu cpu, int offset)
        : base(Read(cpu, offset), Write(cpu, offset)) { }

    public static Func<long, long> ParamMode(int offset)
    {
        var power = 10.Pow(offset + 1);
        return inst => inst / power % 10;
    }

    public new static Func<long> Read(IntCodeCpu cpu, int offset)
    {
        var mode = ParamMode(offset);
        return () =>
        {
            var mem = cpu.Memory;
            var ptr = cpu.Pointer;
            var m = mode(mem[ptr]);
            if (m == 0) return mem[mem[ptr + offset]];
            if (m == 2) return mem[mem[ptr + offset] + cpu.RelativeBase];
            return mem[ptr + offset];
        };
    }
    
    public new static Action<long> Write(IntCodeCpu cpu, int offset)
    {
        var mode = ParamMode(offset);
        return value =>
        {
            var mem = cpu.Memory;
            var ptr = cpu.Pointer;
            var m = mode(mem[ptr]);
            if (m == 0) mem[mem[ptr + offset]] = value;
            else mem[mem[ptr + offset] + cpu.RelativeBase] = value;
        };
    }
}