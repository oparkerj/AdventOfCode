using System;
using AdventToolkit.Utilities.Computer.Builders;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Binders;

public abstract class ConversionBinder<TArch> : IMemBinder<TArch>
    where TArch : IParsable<TArch>
{
    public bool AllowWrite { get; init; } = true;
    public Func<string, bool> DirectValueCondition { get; init; }

    public Mem<TArch> Bind(Cpu<TArch> cpu, string source)
    {
        var binding = TArch.Parse(source, null);
        if (DirectValueCondition?.Invoke(source) == true) return new Mem<TArch>(() => binding, null);
        return new Mem<TArch>(GetReader(cpu, binding), AllowWrite ? GetWriter(cpu, binding) : null);
    }

    public abstract Func<TArch> GetReader(Cpu<TArch> cpu, TArch binding);

    public abstract Action<TArch> GetWriter(Cpu<TArch> cpu, TArch binding);
}

public class IntBinder : ConversionBinder<int>
{
    public override Func<int> GetReader(Cpu<int> cpu, int binding)
    {
        return () => cpu.Memory[binding];
    }

    public override Action<int> GetWriter(Cpu<int> cpu, int binding)
    {
        return value => cpu.Memory[binding] = value;
    }
}

public class LongBinder : ConversionBinder<long>
{
    public override Func<long> GetReader(Cpu<long> cpu, long binding)
    {
        return () => cpu.Memory[binding];
    }

    public override Action<long> GetWriter(Cpu<long> cpu, long binding)
    {
        return value => cpu.Memory[binding] = value;
    }
}