namespace AdventToolkit.Utilities.Computer;

public class Cpu<TArch>
{
    public int Pointer { get; set; }
    public IMemory<TArch> Memory;
    public IPipeline<TArch> Pipeline;
    public IInstructionSet<TArch> InstructionSet;

    // Create a cpu with a standard pipeline
    public static Cpu<TArch> Standard(IMemory<TArch> memory = null)
    {
        return new Cpu<TArch>
        {
            Memory = memory,
            Pipeline = new StandardPipeline<TArch>()
        };
    }

    // Create a standard cpu with a fixed default register count
    public static Cpu<TArch> StandardRegisters(int size) => Standard(new Registers<TArch>(size));

    public virtual void Execute()
    {
        var pipeline = Pipeline;
        while (!pipeline.Tick(this)) { }
    }

    public void JumpRelative(int offsetToNext) => Pipeline.JumpRelative(this, offsetToNext);
    
    public void JumpRelative(long offsetToNext) => Pipeline.JumpRelative(this, (int) offsetToNext);

    public void JumpTo(int next) => Pipeline.JumpTo(this, next);
    
    public void JumpTo(long next) => Pipeline.JumpTo(this, (int) next);
}