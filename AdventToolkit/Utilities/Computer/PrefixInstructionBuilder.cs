using System;
using AdventToolkit.Utilities.Parsing;

namespace AdventToolkit.Utilities.Computer;

public class PrefixInstructionBuilder<TArch> : OpInstructionBuilder<TArch, bool>
    where TArch : IParsable<TArch>
{
    public static PrefixInstructionBuilder<TArch> Default()
    {
        return InitDefault(new PrefixInstructionBuilder<TArch>(), ParseFunc.Of(TArch.Parse));
    }
}