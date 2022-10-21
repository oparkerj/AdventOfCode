using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Computer;

// Provides tools for creating an instruction set, for instructions which are
// some keyword followed by arguments.
// Also provides parsing of a program for the created instruction set.
public class PrefixInstructionBuilder<TArch>
{
    private readonly Dictionary<string, int> _opCodes;
    private readonly List<OpFunc<TArch, OpArgs<TArch, int>>> _actions;
    private readonly List<IOpParser<TArch, int, OpArgs<TArch, int>>> _readers;
    private readonly Dictionary<string, IMemBinder<TArch>> _binders;

    public PrefixInstructionBuilder()
    {
        _opCodes = new Dictionary<string, int>();
        _actions = new List<OpFunc<TArch, OpArgs<TArch, int>>>();
        _readers = new List<IOpParser<TArch, int, OpArgs<TArch, int>>>();
        _binders = new Dictionary<string, IMemBinder<TArch>>();
    }
    
    public Func<string, IEnumerable<string>> Parser { get; set; }

    // Create a default builder
    // The default builder uses a parser that splits an instruction on spaces or commas.
    // It adds memory binders for register or digit values.
    public static PrefixInstructionBuilder<TArch> Default(Func<string, TArch> parser)
    {
        var builder = new PrefixInstructionBuilder<TArch>();
        var delim = new[] {' ', ','};
        builder.Parser = s => s.Split(delim, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        
        builder.AddBinder("r", new RegisterBinder<TArch>());
        builder.AddBinder("d", new DirectValueBinder<TArch>(parser));
        var regOrVal = new RegOrValBinder<TArch>(s => char.IsDigit(s[0]), parser);
        builder.AddBinder("rd", regOrVal);
        builder.AddBinder("dr", regOrVal);

        return builder;
    }

    public void AddBinder(string key, IMemBinder<TArch> binder) => _binders[key] = binder;

    public PrefixInstructionBuilder<TArch> AddOp(string format, OpFunc<TArch, OpArgs<TArch, int>> func)
    {
        var binders = Parser(format).TakeOne(out var prefix).Select(s => _binders[s]).ToArray();
        var id = _opCodes.Count;
        _opCodes[prefix] = id;
        _actions.Add(func);
        _readers.Add(new OpBinder<TArch, int>(binders));
        return this;
    }

    public PrefixInstructionBuilder<TArch> Add(string format, Action<Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> Add(string format, Func<Mem<TArch>, bool> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0]));
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, bool> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0]));
    }
    
    public PrefixInstructionBuilder<TArch> Add(string format, Action<Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0], inst.Args[1]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> Add(string format, Func<Mem<TArch>, Mem<TArch>, bool> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0], inst.Args[1]));
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0], inst.Args[1]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, Mem<TArch>, bool> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0], inst.Args[1]));
    }
    
    public PrefixInstructionBuilder<TArch> Add(string format, Action<Mem<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (_, inst) =>
        {
            action(inst.Args[0], inst.Args[1], inst.Args[2]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> Add(string format, Func<Mem<TArch>, Mem<TArch>, Mem<TArch>, bool> action)
    {
        return AddOp(format, (_, inst) => action(inst.Args[0], inst.Args[1], inst.Args[2]));
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Action<Cpu<TArch>, Mem<TArch>, Mem<TArch>, Mem<TArch>> action)
    {
        return AddOp(format, (cpu, inst) =>
        {
            action(cpu, inst.Args[0], inst.Args[1], inst.Args[2]);
            return true;
        });
    }
    
    public PrefixInstructionBuilder<TArch> AddCpu(string format, Func<Cpu<TArch>, Mem<TArch>, Mem<TArch>, Mem<TArch>, bool> action)
    {
        return AddOp(format, (cpu, inst) => action(cpu, inst.Args[0], inst.Args[1], inst.Args[2]));
    }

    public OpArgs<TArch, int> Parse(Cpu<TArch> cpu, string instruction)
    {
        var args = Parser(instruction).TakeOne(out var prefix).ToArray();
        var id = _opCodes[prefix];
        return _readers[id].Parse(cpu, id, args);
    }

    public IInstructionSet<TArch> BuildInstructionSet()
    {
        var handler = new OpHandlerArray<TArch, OpArgs<TArch, int>>
        {
            OpActions = _actions.ToArray()
        };
        return new OpcodeArray<TArch, int, OpArgs<TArch, int>>
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
        if (set is OpcodeArray<TArch, int, OpArgs<TArch, int>> array)
        {
            array.Instructions = ParseAll(cpu, instructions);
        }
        else throw new Exception("Cpu has an invalid instruction set type.");
    }

    public IInstructionSet<TArch> BuildAndParseAll(Cpu<TArch> cpu, IEnumerable<string> instructions)
    {
        var set = (OpcodeArray<TArch, int, OpArgs<TArch, int>>) BuildInstructionSet();
        set.Instructions = ParseAll(cpu, instructions);
        return set;
    }
}