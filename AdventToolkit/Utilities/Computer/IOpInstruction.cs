namespace AdventToolkit.Utilities.Computer;

// Represents instructions with unique opcodes
public interface IOpInstruction<TOp>
{
    TOp Opcode { get; }
}