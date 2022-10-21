namespace AdventToolkit.Utilities.Computer;

// Runs the cpu by executing an instruction and then incrementing the pointer.
public class StandardPipeline<TArch> : IPipeline<TArch>
{
    public bool Tick(Cpu<TArch> cpu)
    {
        var ready = cpu.InstructionSet.ExecuteNext(cpu);
        cpu.Pointer++;
        return ready;
    }

    public void JumpRelative(Cpu<TArch> cpu, int offsetToNext)
    {
        cpu.Pointer += offsetToNext - 1;
    }

    public void JumpTo(Cpu<TArch> cpu, int next)
    {
        cpu.Pointer = next - 1;
    }
}