using System;

namespace AdventToolkit.Utilities.Computer;

// Represents a readable and possibly writable value of the current instruction.
// If this Mem is bound to a register, it will read and write that register.
// This may have only a reader, for example from a constant number in the instruction.
// TODO make this into interface in .net 7
public class Mem<TArch>
{
    public readonly Func<TArch> Read;
    public readonly Action<TArch> Write;

    public Mem(Func<TArch> read, Action<TArch> write)
    {
        Read = read;
        Write = write;
    }

    public TArch Value
    {
        get => Read();
        set => Write(value);
    }

    public static implicit operator TArch(Mem<TArch> mem) => mem.Value;
}