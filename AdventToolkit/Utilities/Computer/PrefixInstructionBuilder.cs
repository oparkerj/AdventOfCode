using System;

namespace AdventToolkit.Utilities.Computer;

public class PrefixInstructionBuilder<TArch> : OpInstructionBuilder<TArch, bool>
{
    public new static PrefixInstructionBuilder<TArch> Default(Func<string, TArch> parser)
    {
        return InitDefault(new PrefixInstructionBuilder<TArch>(), parser);
    }
}