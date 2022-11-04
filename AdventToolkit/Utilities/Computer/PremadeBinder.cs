namespace AdventToolkit.Utilities.Computer;

public class PremadeBinder<TArch> : IMemBinder<TArch>
{
    public Mem<TArch> Mem;

    public PremadeBinder(Mem<TArch> mem) => Mem = mem;

    public Mem<TArch> Bind(Cpu<TArch> cpu, string source) => Mem;
}