using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Builders;

public delegate TResult CpuFunc<TArch, in TInst, out TResult>(Cpu<TArch> cpu, TInst inst);