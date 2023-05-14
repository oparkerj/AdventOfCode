namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

// Instruction handler specifically for Opcode instructions
public interface IOpInstructionHandler<TArch, TOp, TInst, TResult> : IInstructionHandler<TArch, TInst, TResult>
    where TInst : IOpInstruction<TOp>
{ }