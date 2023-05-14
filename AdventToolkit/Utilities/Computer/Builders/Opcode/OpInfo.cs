namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

public class OpInfo<TArch, TResult>
{
    public CpuFunc<TArch, OpArgs<TArch, int>, TResult> Action { get; set; }
    
    public int ArgCount { get; set; }

    public IOpParser<TArch, int, OpArgs<TArch, int>> Parser;
}