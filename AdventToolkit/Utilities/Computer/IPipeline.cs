namespace AdventToolkit.Utilities.Computer;

// Represents the order of operations of the cpu.
// Typically: Execute next instruction, increment pointer.
public interface IPipeline<TArch>
{
    // If this returns false, then the CPU should halt
    bool Tick(Cpu<TArch> cpu);

    void JumpRelative(Cpu<TArch> cpu, int offsetToNext);

    void JumpTo(Cpu<TArch> cpu, int next);
}