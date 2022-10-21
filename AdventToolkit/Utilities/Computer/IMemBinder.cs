namespace AdventToolkit.Utilities.Computer;

// A converter which can turn a string, such as a register,
// into a Mem object, which can read/write that register.
// A Mem object can also represent a constant value.
public interface IMemBinder<TArch>
{
    Mem<TArch> Bind(Cpu<TArch> cpu, string source);
}