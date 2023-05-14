using System;
using System.Collections.Generic;
using AdventToolkit.Utilities.Computer.Builders;

namespace AdventToolkit.Utilities.Computer.Core;

public class NodeSet<TArch, TKey, TNode> : IInstructionSet<TArch>
    where TNode : INode<TKey>
{
    private readonly Dictionary<TKey, TNode> _nodes;

    public NodeSet() => _nodes = new Dictionary<TKey, TNode>();

    public IInstructionHandler<TArch, TNode, TArch> Handler;

    public bool ExecuteNext(Cpu<TArch> cpu) => throw new NotSupportedException();

    public void Add(TNode node) => this[node.Key] = node;

    public TArch GetValue(Cpu<TArch> cpu, TNode node) => Handler.Handle(cpu, node);

    public TNode this[TKey key]
    {
        get => _nodes[key];
        set => _nodes[key] = value;
    }
}