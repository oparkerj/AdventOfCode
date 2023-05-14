using System;
using System.Numerics;
using AdventToolkit.Utilities.Computer.Binders;
using AdventToolkit.Utilities.Computer.Builders;
using AdventToolkit.Utilities.Computer.Builders.Opcode;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Extensions;

public static class CpuExtensions
{
    public static void JumpRelative<T>(this Cpu<T> cpu, long offset)
    {
        cpu.JumpRelative((int) offset);
    }
    
    public static void JumpRelative<T>(this Cpu<T> cpu, BigInteger offset)
    {
        cpu.JumpRelative((int) offset);
    }

    public static void AddDefaultRegisterBinders<TArch, TBind, T>(this OpInstructionBuilder<TArch, T> builder)
        where TArch : IParsable<TArch>
        where TBind : ConversionBinder<TArch>, new()
    {
        builder.AddBinder("r", new TBind());
        builder.AddBinder("d", new TBind {DirectValueCondition = _ => true, AllowWrite = false});
        var binder = new TBind
        {
            DirectValueCondition = s => s[0] is '-' or '+' ? char.IsDigit(s[1]) : char.IsDigit(s[0])
        };
        builder.AddBinder("rd", binder);
        builder.AddBinder("dr", binder);
    }
    
    public static void AddDefaultRegisterBinders<T>(this OpInstructionBuilder<int, T> builder)
    {
        AddDefaultRegisterBinders<int, IntBinder, T>(builder);
    }
    
    public static void AddDefaultRegisterBinders<T>(this OpInstructionBuilder<long, T> builder)
    {
        AddDefaultRegisterBinders<long, LongBinder, T>(builder);
    }

    public static void ChangeOp<TArch, TOp, TResult>(this OpcodeArray<TArch, TOp, OpArgs<TArch, TOp>, TResult> array, int index, TOp op)
    {
        array.Instructions[index].Opcode = op;
    }

    private static void SetupMemFilter<TArch, TResult>(OpInstructionBuilder<TArch, TResult> builder)
        where TArch : struct, IBinaryInteger<TArch>
    {
        builder.SetMemFilter(builder.Count, mem => new MemNumber<TArch>(mem));
    }

    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, mem => action((MemNumber<TArch>) mem));
    }
    
    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, mem => action((MemNumber<TArch>) mem));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<Cpu<TArch>, MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, mem) => action(cpu, (MemNumber<TArch>) mem));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<Cpu<TArch>, MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, mem) => action(cpu, (MemNumber<TArch>) mem));
    }
    
    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<MemNumber<TArch>, MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, (a, b) => action((MemNumber<TArch>) a, (MemNumber<TArch>) b));
    }
    
    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<MemNumber<TArch>, MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, (a, b) => action((MemNumber<TArch>) a, (MemNumber<TArch>) b));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<Cpu<TArch>, MemNumber<TArch>, MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, a, b) => action(cpu, (MemNumber<TArch>) a, (MemNumber<TArch>) b));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<Cpu<TArch>, MemNumber<TArch>, MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, a, b) => action(cpu, (MemNumber<TArch>) a, (MemNumber<TArch>) b));
    }
    
    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<MemNumber<TArch>, MemNumber<TArch>, MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, (a, b, c) => action((MemNumber<TArch>) a, (MemNumber<TArch>) b, (MemNumber<TArch>) c));
    }
    
    public static void AddNum<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<MemNumber<TArch>, MemNumber<TArch>, MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.Add(format, (a, b, c) => action((MemNumber<TArch>) a, (MemNumber<TArch>) b, (MemNumber<TArch>) c));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Action<Cpu<TArch>, MemNumber<TArch>, MemNumber<TArch>, MemNumber<TArch>> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, a, b, c) => action(cpu, (MemNumber<TArch>) a, (MemNumber<TArch>) b, (MemNumber<TArch>) c));
    }
    
    public static void AddNumCpu<TArch, TResult>(this OpInstructionBuilder<TArch, TResult> builder, string format, Func<Cpu<TArch>, MemNumber<TArch>, MemNumber<TArch>, MemNumber<TArch>, TResult> action)
        where TArch : struct, IBinaryInteger<TArch>
    {
        SetupMemFilter(builder);
        builder.AddCpu(format, (cpu, a, b, c) => action(cpu, (MemNumber<TArch>) a, (MemNumber<TArch>) b, (MemNumber<TArch>) c));
    }
}