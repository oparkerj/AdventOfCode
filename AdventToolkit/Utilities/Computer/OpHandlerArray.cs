namespace AdventToolkit.Utilities.Computer;

// Stores the functions that executes instructions when the opcode is an
// array index
public class OpHandlerArray<TArch, TInst, TResult> : IOpInstructionHandler<TArch, int, TInst, TResult>
    where TInst : IOpInstruction<int>
{
    public CpuFunc<TArch, TInst, TResult>[] OpActions;

    public TResult Handle(Cpu<TArch> cpu, TInst inst)
    {
        return OpActions[inst.Opcode](cpu, inst);
    }
}