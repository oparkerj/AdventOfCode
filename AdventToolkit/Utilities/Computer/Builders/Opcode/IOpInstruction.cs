namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

// Represents instructions with unique opcodes
public interface IOpInstruction<TOp>
{
    TOp Opcode { get; }
}