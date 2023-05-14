namespace AdventToolkit.Utilities.Computer.Core;

public class NodeCpu<TArch, TKey, TNode> : Cpu<TArch>
    where TNode : INode<TKey>
{
    public NodeSet<TArch, TKey, TNode> NodeSet
    {
        get => (NodeSet<TArch, TKey, TNode>) InstructionSet;
        set => InstructionSet = value;
    }
}