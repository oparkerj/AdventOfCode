namespace AdventToolkit.Utilities.Computer;

public delegate TResult CpuFunc<TArch, in TInst, out TResult>(Cpu<TArch> cpu, TInst inst);