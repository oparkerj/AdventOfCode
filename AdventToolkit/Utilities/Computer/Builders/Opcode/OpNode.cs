using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

public class OpNode<TArch, TOp, TKey> : OpArgs<TArch, TOp>, INode<TKey>
{
    public TKey Key { get; init; }
}