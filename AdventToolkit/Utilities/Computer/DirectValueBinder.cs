using System;

namespace AdventToolkit.Utilities.Computer;

// Binder which parses the argument into a constant value
public class DirectValueBinder<TArch> : IMemBinder<TArch>
{
    // TODO .net 7 replace with IParseable
    private readonly Func<string, TArch> _parser;

    public DirectValueBinder(Func<string, TArch> parser) => _parser = parser;

    public Mem<TArch> Bind(Cpu<TArch> cpu, string source)
    {
        var value = _parser(source);
        return new Mem<TArch>(() => value, null);
    }
}