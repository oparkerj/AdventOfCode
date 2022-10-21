namespace AdventToolkit.Utilities.Computer;

// Represents an instruction that is identified by an opcode
// and has memory bindings for the arguments.
public class OpArgs<TArch, TOp> : IOpInstruction<TOp>
{
    public TOp Opcode { get; set; }
    public Mem<TArch>[] Args;
}