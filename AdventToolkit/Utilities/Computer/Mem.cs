using System;

namespace AdventToolkit.Utilities.Computer;

// Represents a readable and possibly writable value of the current instruction.
// If this Mem is bound to a register, it will read and write that register.
// This may have only a reader, for example from a constant number in the instruction.
public class Mem<TArch>
{
    public readonly Func<TArch> Read;
    public readonly Action<TArch> Write;

    public Mem(Func<TArch> read, Action<TArch> write)
    {
        Read = read;
        Write = write;
    }

    public static Mem<T> Const<T>(T value) => new(() => value, null);

    public TArch Value
    {
        get => Read();
        set => Write(value);
    }

    public static implicit operator TArch(Mem<TArch> mem) => mem.Value;

    public Mem<TArch> When(Func<TArch, bool> condition, Action<TArch> action)
    {
        var value = Value;
        if (condition(value)) action(value);
        return this;
    }
    
    public Mem<TArch> When(Func<TArch, bool> condition, Action<TArch> action, TArch actionValue)
    {
        var value = Value;
        if (condition(value)) action(actionValue);
        return this;
    }
    
    public Mem<TArch> When(Func<TArch, bool> condition, Action<TArch> action, Mem<TArch> actionValue)
    {
        var value = Value;
        if (condition(value)) action(actionValue);
        return this;
    }
    
    public Mem<TArch> When(TArch value, Action<TArch> action)
    {
        if (Equals(Value, value)) action(value);
        return this;
    }
    
    public Mem<TArch> When(TArch value, Action<TArch> action, TArch actionValue)
    {
        if (Equals(Value, value)) action(actionValue);
        return this;
    }
    
    public Mem<TArch> When(TArch value, Action<TArch> action, Mem<TArch> actionValue)
    {
        if (Equals(Value, value)) action(actionValue);
        return this;
    }
}