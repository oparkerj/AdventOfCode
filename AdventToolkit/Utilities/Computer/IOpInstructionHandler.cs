namespace AdventToolkit.Utilities.Computer;

// Instruction handler specifically for Opcode instructions
public interface IOpInstructionHandler<TArch, TOp, TInst> : IInstructionHandler<TArch, TInst>
    where TInst : IOpInstruction<TOp>
{ }

public delegate bool OpFunc<TArch, in TInst>(Cpu<TArch> cpu, TInst inst);