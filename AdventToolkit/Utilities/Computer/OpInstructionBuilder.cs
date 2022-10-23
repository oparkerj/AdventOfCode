using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Computer;

// Provides tools for creating an instruction set, for instructions which are
// some keyword followed by arguments.
// Also provides parsing of a program for the created instruction set.
public class OpInstructionBuilder<TArch, TResult>
{
    protected readonly Dictionary<string, int> _opCodes;
    protected readonly List<CpuFunc<TArch, OpArgs<TArch, int>, TResult>> _actions;
    protected readonly List<IOpParser<TArch, int, OpArgs<TArch, int>>> _readers;
    protected readonly Dictionary<string, IMemBinder<TArch>> _binders;

    public OpInstructionBuilder()
    {
        _opCodes = new Dictionary<string, int>();
        _actions = new List<CpuFunc<TArch, OpArgs<TArch, int>, TResult>>();
        _readers = new List<IOpParser<TArch, int, OpArgs<TArch, int>>>();
        _binders = new Dictionary<string, IMemBinder<TArch>>();
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
        var builder = new OpInstructionBuilder<TArch, TResult>();
        builder.Splitter = s => s.SplitSpaceOrComma();
        builder.OpSelector = (_, _) => (0, null);
        builder.ArgSelector = (_, list, _) => list.Skip(1);
        builder.ParserSelector = (_, _, args, binders) => new OpBinder<TArch, int>(binders.GetValues(args).ToArray());
        
        builder.AddDefaultRegisterBinders(parser);

        return builder;
    }

    public void AddDefaultRegisterBinders(Func<string, TArch> parser)
    {
        AddBinder("r", new RegisterBinder<TArch>());
        AddBinder("d", new DirectValueBinder<TArch>(parser));
        var regOrVal = new RegOrValBinder<TArch>(s => char.IsDigit(s[0]), parser);
        AddBinder("rd", regOrVal);
        AddBinder("dr", regOrVal);
    }

    public void AddBinder(string key, IMemBinder<TArch> binder) => _binders[key] = binder;

    protected virtual void AddOp((int, string) opInfo, IList<string> parts, CpuFunc<TArch, OpArgs<TArch, int>, TResult> action)
    {
        var (_, opcode) = opInfo;
        var args = ArgSelector(opcode, parts, true);
        var id = _opCodes.Count;
        _opCodes[opcode] = id;
        _actions.Add(action);
        _readers.Add(ParserSelector(id, opcode, args, _binders));
    }

    public OpInstructionBuilder<TArch, TResult> AddOp(string format, CpuFunc<TArch, OpArgs<TArch, int>, TResult> action)
    {
        var parts = Splitter(format).AsArray();
        var (opIndex, opText) = OpSelector(parts, true);
        opText ??= parts[opIndex];
        AddOp((opIndex, opText), parts, action);
        return this;
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

    public OpArgs<TArch, int> Parse(Cpu<TArch> cpu, string instruction)
    {
        var parts = Splitter(instruction).AsArray();
        var (opIndex, _) = OpSelector(parts, false);
        var op = parts[opIndex];
        var args = ArgSelector(op, parts, false);
        var id = _opCodes[op];
        return _readers[id].Parse(cpu, id, args);
    }

    public IInstructionSet<TArch> BuildInstructionSet()
    {
        var handler = new OpHandlerArray<TArch, OpArgs<TArch, int>, TResult>
        {
            OpActions = _actions.ToArray()
        };
        return new OpcodeArray<TArch, int, OpArgs<TArch, int>, TResult>
        {
            InstructionHandler = handler
        };
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