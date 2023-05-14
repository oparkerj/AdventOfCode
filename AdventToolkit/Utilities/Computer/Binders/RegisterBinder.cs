using AdventToolkit.Utilities.Computer.Builders;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Binders;

// Binds the argument to a cpu register
public class RegisterBinder<TArch> : IMemBinder<TArch>
{
    public Mem<TArch> Bind(Cpu<TArch> cpu, string source)
    {
        return new Mem<TArch>(() => cpu.Memory[source], value => cpu.Memory[source] = value);
    }
}