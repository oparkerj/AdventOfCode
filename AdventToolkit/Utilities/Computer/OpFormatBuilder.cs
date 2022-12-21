using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Computer;

public class OpFormatBuilder<TArch> : OpInstructionBuilder<TArch, TArch>
{
    private readonly HashSet<(int, string)> _opTypes;
    private readonly Dictionary<string, NodeParser> _argSelectors;

    public OpFormatBuilder()
    {
        _opTypes = new HashSet<(int, string)>();
        _argSelectors = new Dictionary<string, NodeParser>();
    }

    public new static OpFormatBuilder<TArch> Default(Func<string, TArch> parser)
    {
        var builder = new OpFormatBuilder<TArch>();
        builder.Splitter = s => s.SplitSpaceOrComma();
        builder.OpSelector = builder.FindOpcode;
        builder.ArgSelector = builder.GetArgs;
        builder.ParserSelector = builder.GetParser;
        
        builder.AddDefaultRegisterBinders(parser);

        return builder;
    }

    protected override void AddOp((int, string) opInfo, IList<string> parts, CpuFunc<TArch, OpArgs<TArch, int>, TArch> action)
    {
        var nameIndex = parts.IndexOf("()");
        var parser = new NodeParser();
        parser.Name = nameIndex;
        parser.Args = Enumerable.Range(0, parts.Count).Where(i => i != nameIndex && i != opInfo.Item1 && parts[i] != "_").ToArray();
        _opTypes.Add(opInfo);
        _argSelectors[opInfo.Item2] = parser;
        
        base.AddOp(opInfo, parts, action);
    }

    private (int, string) FindOpcode(IList<string> parts, bool setup)
    {
        if (setup)
        {
            var i = parts.FirstIndex(s => s.StartsWith('{') && s.EndsWith('}'));
            if (i < 0) return (-1, "");
            return (i, parts[i][1..^1]);
        }
        
        for (var i = 0; i < parts.Count; i++)
        {
            if (_opTypes.Contains((i, parts[i]))) return (i, parts[i]);
        }
        return (-1, null);
    }

    private IEnumerable<string> GetArgs(string opcode, IList<string> parts, bool setup)
    {
        return _argSelectors[opcode].GetArgs(parts, setup);
    }

    private IOpParser<TArch, int, OpArgs<TArch, int>> GetParser(int op, string opcode, IEnumerable<string> args, Dictionary<string, IMemBinder<TArch>> binders)
    {
        var selector = _argSelectors[opcode];
        selector.Binders = binders.GetValues(args).ToArray();
        return selector;
    }

    private OpNode<TArch, int, string> GetNode(OpArgs<TArch, int> args) => (OpNode<TArch, int, string>) args;

    public OpInstructionBuilder<TArch, TArch> AddKey(string format, Func<string, TArch> action)
    {
        return AddOp(format, (_, inst) => action(GetNode(inst).Key));
    }
    
    public OpInstructionBuilder<TArch, TArch> AddKey(string format, Func<string, Mem<TArch>, TArch> action)
    {
        return AddOp(format, (_, inst) => action(GetNode(inst).Key, inst.Args[0]));
    }
    
    public OpInstructionBuilder<TArch, TArch> AddKey(string format, Func<string, Mem<TArch>, Mem<TArch>, TArch> action)
    {
        return AddOp(format, (_, inst) => action(GetNode(inst).Key, inst.Args[0], inst.Args[1]));
    }
    
    public OpInstructionBuilder<TArch, TArch> AddKey(string format, Func<string, Mem<TArch>, Mem<TArch>, Mem<TArch>, TArch> action)
    {
        return AddOp(format, (_, inst) => action(GetNode(inst).Key, inst.Args[0], inst.Args[1], inst.Args[2]));
    }

    public NodeCpu<TArch, string, OpNode<TArch, int, string>> ParseNodes(IEnumerable<string> nodes)
    {
        var cpu = new NodeCpu<TArch, string, OpNode<TArch, int, string>>();
        cpu.NodeSet = new NodeSet<TArch, string, OpNode<TArch, int, string>>();
        
        var handler = new OpHandlerArray<TArch, OpNode<TArch, int, string>, TArch>();
        handler.OpActions = _actions.Select(func => new CpuFunc<TArch, OpNode<TArch, int, string>, TArch>(func)).ToArray(_actions.Count);
        cpu.NodeSet.Handler = handler;
        
        cpu.Memory = new LookupCache<TArch, string>(s =>
        {
            var nodes = cpu.NodeSet;
            var node = nodes[s];
            return nodes.GetValue(cpu, node);
        });
        foreach (var opArgs in ParseAll(cpu, nodes))
        {
            var node = (OpNode<TArch, int, string>) opArgs;
            cpu.NodeSet.Add(node);
        }
        return cpu;
    }

    public class NodeParser : OpBinder<TArch, int>
    {
        public int Name;
        public int[] Args;

        public IEnumerable<string> GetArgs(IList<string> parts, bool setup)
        {
            if (!setup) yield return parts[Name];
            foreach (var arg in Args)
            {
                yield return parts[arg];
            }
        }

        public override OpArgs<TArch, int> Parse(Cpu<TArch> cpu, int op, IEnumerable<string> args)
        {
            args = args.PullOne(out var key);
            var argBinds = GetBinds(cpu, Binders, args);
            return new OpNode<TArch, int, string>
            {
                Key = key,
                Opcode = op,
                Args = argBinds,
            };
        }
    }
}