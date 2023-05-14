using System;
using AdventToolkit.Utilities.Computer.Builders;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Binders;

public class ConstBinder<TArch> : IMemBinder<TArch>
{
    private Func<Cpu<TArch>, Mem<TArch>> _binder;

    public ConstBinder(TArch c)
    {
        _binder = cpu => new Mem<TArch>(() => cpu.Memory[c], arch => cpu.Memory[c] = arch);
    }

    public ConstBinder(int c)
    {
        _binder = cpu => new Mem<TArch>(() => cpu.Memory[c], arch => cpu.Memory[c] = arch);
    }

    public ConstBinder(char c)
    {
        _binder = cpu => new Mem<TArch>(() => cpu.Memory[c], arch => cpu.Memory[c] = arch);
    }

    public ConstBinder(long c)
    {
        _binder = cpu => new Mem<TArch>(() => cpu.Memory[c], arch => cpu.Memory[c] = arch);
    }

    public ConstBinder(string c)
    {
        _binder = cpu => new Mem<TArch>(() => cpu.Memory[c], arch => cpu.Memory[c] = arch);
    }

    public Mem<TArch> Bind(Cpu<TArch> cpu, string _) => _binder(cpu);
}