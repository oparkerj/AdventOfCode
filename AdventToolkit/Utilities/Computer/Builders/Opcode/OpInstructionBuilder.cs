using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Computer.Binders;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventToolkit.Utilities.Computer.Builders.Opcode;

// Provides tools for creating an instruction set, for instructions which are
// some keyword followed by arguments.
// Also provides parsing of a program for the created instruction set.
public class OpInstructionBuilder<TArch, TResult>
{
    protected readonly Dictionary<string, int> _opCodes;
    protected readonly List<OpInfo<TArch, TResult>> _ops;
    protected readonly Dictionary<string, IMemBinder<TArch>> _binders;
    protected readonly Dictionary<int, Func<Mem<TArch>, Mem<TArch>>> _memFilters; // opIndex to mem Filter

    public OpInstructionBuilder()
    {
        _opCodes = new Dictionary<string, int>();
        _ops = new List<OpInfo<TArch, TResult>>();
        _binders = new Dictionary<string, IMemBinder<TArch>>();
        _memFilters = new Dictionary<int, Func<Mem<TArch>, Mem<TArch>>>();
    }

    public Func<string, IEnumerable<string>> Splitter { get; set; }
    public Func<IList<string>, bool, (int, string)> OpSelector { get; set; }
    public Func<string, IList<string>, bool, IEnumerable<string>> ArgSelector { get; set; }
    public Func<int, string, IEnumerable<string>, Dictionary<string, IMemBinder<TArch>>, IOpParser<TArch, int, OpArgs<TArch, int>>> ParserSelector { get; set; }

    // Create a default builder
    // The default builder uses a parser that splits an instruction on spaces or commas.
    // It adds memory binders for register or digit values.
    public static OpInstructionBuilder<TArch, TResult> Default(Func<string, TArch> parser)
    {
        return InitDefault(new OpInstructionBuilder<TArch, TResult>(), parser);
    }

    public static T InitDefault<T>(T builder, Func<string, TArch> parser)
        where T : OpInstructionBuilder<TArch, TResult>
    {
        builder.Splitter = s => s.SplitSpaceOrComma();
        builder.OpSelector = (_, _) => (0, null);
        builder.ArgSelector = (_, list, _) => list.Skip(1);
        builder.ParserSelector = (_, _, args, binders) => new OpBinder<TArch, int>(binders.GetValues(args).ToArray());
        
        builder.AddDefaultRegisterBinders(parser);

        return builder;
    }

    public int Count => _opCodes.Count;

    public void ClearBinders() => _binders.Clear();

    public void AddDefaultRegisterBinders(Func<string, TArch> parser)
    {
        AddBinder("r", new RegisterBinder<TArch>());
        AddBinder("d", new DirectValueBinder<TArch>(parser));
        var regOrVal = new RegOrValBinder<TArch>(s => s[0] is '-' or '+' ? char.IsDigit(s[1]) : char.IsDigit(s[0]), parser);
        AddBinder("rd", regOrVal);
        AddBinder("dr", regOrVal);
    }

    public void AddBinder(string key, IMemBinder<TArch> binder) => _binders[key] = binder;

    public void AddBinder(string key, Mem<TArch> mem) => AddBinder(key, new PremadeBinder<TArch>(mem));
    
    public void SetMemFilter(int opIndex, Func<Mem<TArch>, Mem<TArch>> filter)
    {
        _memFilters[opIndex] = filter;
    }

    protected virtual void AddOp((int, string) opInfo, IList<string> parts, CpuFunc<TArch, OpArgs<TArch, int>, TResult> action)
    {
        var (_, opcode) = opInfo;
        var args = ArgSelector(opcode, parts, true).ToList();
        var id = _opCodes.Count;
        _opCodes[opcode] = id;
        _ops.Add(new OpInfo<TArch, TResult>
        {
            Action = action,
            ArgCount = args.Count,
            Parser = ParserSelector(id, opcode, args, _binders)
        });
    }

    public OpInstructionBuilder<TArch, TResult> AddOp(string format, CpuFunc<TArch, OpArgs<TArch, int>, TResult> action)
    {
        var parts = Splitter(format).AsArray();
        var (opIndex, opText) = OpSelector(parts, true);
        opText ??= parts[opIndex];
        AddOp((opIndex, opText), parts, action);
        return this;
    }

    public OpInstructionBuilder<TArch, TResult> Add(string format)
    {
        return AddOp(format, (_, _) => default);
    }

    public OpInstructionBuilder<TArch, TResult> Add(string format, Action action)
    {
        return AddOp(format, (_, _) =>
        {
            action();
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Func<TResult> action)
    {
        return AddOp(format, (_, _) => action());
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Action<Cpu<TArch>> action)
    {
        return AddOp(format, (cpu, _) =>
        {
            action(cpu);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Func<Cpu<TArch>, TResult> action)
    {
        return AddOp(format, (cpu, _) => action(cpu));
    }

    public OpInstructionBuilder<TArch, TResult> Add(string format, Action<Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Func<Mem<TArch>, TResult> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0]));
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, TResult> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0]));
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Action<Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0], inst.Args[1]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Func<Mem<TArch>, Mem<TArch>, TResult> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0], inst.Args[1]));
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0], inst.Args[1]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, Mem<TArch>, TResult> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0], inst.Args[1]));
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Action<Mem<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0], inst.Args[1], inst.Args[2]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> Add(string format, Func<Mem<TArch>, Mem<TArch>, Mem<TArch>, TResult> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0], inst.Args[1], inst.Args[2]));
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0], inst.Args[1], inst.Args[2]);
            return default;
        });
    }
    
    public OpInstructionBuilder<TArch, TResult> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, Mem<TArch>, Mem<TArch>, TResult> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0], inst.Args[1], inst.Args[2]));
    }

    private void FilterArgs(OpArgs<TArch, int> inst)
    {
        if (!_memFilters.TryGetValue(inst.Opcode, out var filter)) return;
        var mem = inst.Args;
        for (var i = 0; i < mem.Length; i++)
        {
            mem[i] = filter(mem[i]);
        }
    }

    public OpArgs<TArch, int> Parse(Cpu<TArch> cpu, string instruction)
    {
        var parts = Splitter(instruction).AsArray();
        var (opIndex, _) = OpSelector(parts, false);
        var op = opIndex >= 0 ? parts[opIndex] : "";
        var args = ArgSelector(op, parts, false);
        var id = _opCodes[op];
        var inst = _ops[id].Parser.Parse(cpu, id, args);
        FilterArgs(inst);
        return inst;
    }
    
    public OpArgs<TArch, int> ParseAsOp(Cpu<TArch> cpu, string instruction, int opIndex)
    {
        var parts = Splitter(instruction).AsArray();
        var op = _opCodes.WhereValue(opIndex).First().Key;
        var args = ArgSelector(op, parts, false);
        var inst = _ops[opIndex].Parser.Parse(cpu, opIndex, args);
        FilterArgs(inst);
        return inst;
    }
    
    // TODO .net7 parse expression

    public IEnumerable<int> ArgCounts => _ops.Select(info => info.ArgCount);

    public int OpIndex(string op) => _opCodes[op];

    public int ArgCount(int opIndex) => _ops[opIndex].ArgCount;

    public IEnumerable<CpuFunc<TArch, OpArgs<TArch, int>, TResult>> Actions => _ops.Select(info => info.Action);

    public IInstructionSet<TArch> BuildInstructionSet()
    {
        var handler = new OpHandlerArray<TArch, OpArgs<TArch, int>, TResult>
        {
            OpActions = Actions.ToArray()
        };
        return new OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult>
        {
            InstructionHandler = handler
        };
    }

    public OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult> GetInstructionSet(Cpu<TArch> cpu)
    {
        return (OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult>) cpu.InstructionSet;
    }

    public OpHandlerArray<TArch, OpArgs<TArch, int>, TResult> GetInstructionHandler(Cpu<TArch> cpu)
    {
        return (OpHandlerArray<TArch, OpArgs<TArch, int>, TResult>) GetInstructionSet(cpu).InstructionHandler;
    }

    public OpArgs<TArch, int>[] ParseAll(Cpu<TArch> cpu, IEnumerable<string> instructions)
    {
        return instructions.Select(s => Parse(cpu, s)).ToArray();
    }

    public void ParseAllInto(Cpu<TArch> cpu, IEnumerable<string> instructions)
    {
        var set = cpu.InstructionSet;
        if (set is OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult> array)
        {
            array.Instructions = ParseAll(cpu, instructions);
        }
        else throw new Exception("Cpu has an invalid instruction set type.");
    }

    public IInstructionSet<TArch> BuildAndParseAll(Cpu<TArch> cpu, IEnumerable<string> instructions)
    {
        var set = (OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult>) BuildInstructionSet();
        set.Instructions = ParseAll(cpu, instructions);
        return set;
    }
}