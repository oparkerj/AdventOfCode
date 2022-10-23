namespace AdventToolkit.Utilities.Computer;

// Contains a handler that can execute instructions for given opcodes
// and also stores the program being executed.
public class OpcodeArray<TArch, TOp, TInst, TResult> : IInstructionSet<TArch>
    where TInst : IOpInstruction<TOp>
{
    public IOpInstructionHandler<TArch, TOp, TInst, TResult> InstructionHandler;
    public TInst[] Instructions;

    public bool ExecuteNext(Cpu<TArch> cpu)
    {
        var ptr = cpu.Pointer;
        if (ptr < 0 || ptr >= Instructions.Length)
        {
            // Could perform custom action if pointer is out of bounds
            return true;
        }
        var result = InstructionHandler.Handle(cpu, Instructions[cpu.Pointer]);
        return result is true;
    }
}