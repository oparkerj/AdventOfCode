using System.Numerics;
using AdventToolkit.Utilities.Computer;

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
}