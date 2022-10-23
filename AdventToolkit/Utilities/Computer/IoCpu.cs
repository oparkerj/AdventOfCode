using System;

namespace AdventToolkit.Utilities.Computer;

public class IoCpu<TArch> : Cpu<TArch>
{
    // ReSharper disable once InconsistentNaming
    public Mem<TArch> IO { get; private set; }

    public void SetInput(Func<TArch> input) => IO = new Mem<TArch>(input, IO?.Write);

    public void SetOutput(Action<TArch> output) => IO = new Mem<TArch>(IO?.Read, output);
}