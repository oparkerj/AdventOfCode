using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Computer;

// Given an array of memory binders, constructs an opcode instruction
// from the opcode and its arguments.
public class OpBinder<TArch, TOp> : IOpParser<TArch, TOp, OpArgs<TArch, TOp>>
{
    public IMemBinder<TArch>[] Binders;

    public OpBinder(params IMemBinder<TArch>[] binders) => Binders = binders;

    public static Mem<TArch>[] GetBinds(Cpu<TArch> cpu, IMemBinder<TArch>[] binders, IEnumerable<string> args)
    {
        return binders.Zip(args)
            .Select(tuple => tuple.First.Bind(cpu, tuple.Second))
            .ToArray(binders.Length);
    }

    public virtual OpArgs<TArch, TOp> Parse(Cpu<TArch> cpu, TOp op, IEnumerable<string> args)
    {
        var argBinds = GetBinds(cpu, Binders, args);
        return new OpArgs<TArch, TOp> {Opcode = op, Args = argBinds};
    }
}