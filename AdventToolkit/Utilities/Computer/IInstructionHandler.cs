namespace AdventToolkit.Utilities.Computer;

// Represents an object that can carry out the operation of the 
// specified instruction.
public interface IInstructionHandler<TArch, in TInst, out TResult>
{
    TResult Handle(Cpu<TArch> cpu, TInst inst);
}