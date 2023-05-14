using System;
using AdventToolkit.Utilities.Computer.Builders;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Binders;

// Parses the argument into a constant value or binds to a register, depending on the value.
public class RegOrValBinder<TArch> : IMemBinder<TArch>
{
    private readonly Func<string, bool> _isDirect;
    private readonly DirectValueBinder<TArch> _directValue;
    private readonly RegisterBinder<TArch> _register;

    public RegOrValBinder(Func<string, bool> isDirect, Func<string, TArch> parser)
    {
        _isDirect = isDirect;
        _directValue = new DirectValueBinder<TArch>(parser);
        _register = new RegisterBinder<TArch>();
    }

    public Mem<TArch> Bind(Cpu<TArch> cpu, string source)
    {
        return _isDirect(source) ? _directValue.Bind(cpu, source) : _register.Bind(cpu, source);
    }
}