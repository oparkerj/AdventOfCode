namespace AdventToolkit.Utilities.Computer.Core;

// Represents the set of instructions that the cpu can execute.
// Knows how to execute the next instruction for the cpu.
public interface IInstructionSet<TArch>
{
    bool ExecuteNext(Cpu<TArch> cpu);
}