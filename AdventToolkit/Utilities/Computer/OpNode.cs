namespace AdventToolkit.Utilities.Computer;

public class OpNode<TArch, TOp, TKey> : OpArgs<TArch, TOp>, INode<TKey>
{
    public TKey Key { get; init; }
}