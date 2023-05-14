using System.Collections.Generic;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

// Converter to parse an opcode and arguments into an instruction.
public interface IOpParser<TArch, in TOp, out TInst>
    where TInst : IOpInstruction<TOp>
{
    TInst Parse(Cpu<TArch> cpu, TOp op, IEnumerable<string> args);
}