namespace AdventToolkit.Utilities.Computer;

// Stores the functions that executes instructions when the opcode is an
// array index
public class OpHandlerArray<TArch, TInst> : IOpInstructionHandler<TArch, int, TInst>
    where TInst : IOpInstruction<int>
{
    public OpFunc<TArch, TInst>[] OpActions;

    public bool Handle(Cpu<TArch> cpu, TInst inst)
    {
        return OpActions[inst.Opcode](cpu, inst);
    }
}