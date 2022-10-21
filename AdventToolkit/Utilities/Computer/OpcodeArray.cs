namespace AdventToolkit.Utilities.Computer;

// Contains a handler that can execute instructions for given opcodes
// and also stores the program being executed.
public class OpcodeArray<TArch, TOp, TInst> : IInstructionSet<TArch>
    where TInst : IOpInstruction<TOp>
{
    public IOpInstructionHandler<TArch, TOp, TInst> InstructionHandler;
    public TInst[] Instructions;

    public bool ExecuteNext(Cpu<TArch> cpu)
    {
        var ptr = cpu.Pointer;
        if (ptr < 0 || ptr >= Instructions.Length)
        {
            // Could perform custom action if pointer is out of bounds
            return false;
        }
        return InstructionHandler.Handle(cpu, Instructions[cpu.Pointer]);
    }
}