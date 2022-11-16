namespace AdventToolkit.Utilities.Computer;

// Runs the cpu by executing an instruction and then incrementing the pointer.
public class StandardPipeline<TArch> : IPipeline<TArch>
{
    public virtual bool Tick(Cpu<TArch> cpu)
    {
        var halt = cpu.InstructionSet.ExecuteNext(cpu);
        if (!halt) cpu.Pointer++;
        return halt;
    }

    public virtual void JumpRelative(Cpu<TArch> cpu, int offsetToNext)
    {
        cpu.Pointer += offsetToNext - 1;
    }

    public virtual void JumpTo(Cpu<TArch> cpu, int next)
    {
        cpu.Pointer = next - 1;
    }
}