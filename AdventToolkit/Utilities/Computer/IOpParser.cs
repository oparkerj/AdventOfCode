using System.Collections.Generic;

namespace AdventToolkit.Utilities.Computer;

// Converter to parse an opcode and arguments into an instruction.
public interface IOpParser<TArch, in TOp, out TInst>
    where TInst : IOpInstruction<TOp>
{
    TInst Parse(Cpu<TArch> cpu, TOp op, IEnumerable<string> args);
}