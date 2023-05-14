using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using AdventToolkit.Utilities.Computer.Builders.Opcode;
using AdventToolkit.Utilities.Computer.Core;

namespace AdventOfCode2015.Puzzles;

public class Day7 : Puzzle<int>
{
    public Dictionary<string, Signal> Wires = new();
    public ushort BOverride;

    public ushort GetSignal(string from)
    {
        if (Part == 2 && from == "b") return BOverride;
        return char.IsNumber(from[0]) ? ushort.Parse(from) : Wires[from].Data;
    }

    public void ReadWires()
    {
        var signals = GetSignal;
        signals = Data.Memoize(signals);
        
        var matcher = new StringMatcher();
        matcher.AddRegex<string, string>(@"^(\S+) -> (\S+)$", (a, b) => Wires[b] = new Pass(a) {Signals = signals});
        matcher.AddRegex<string, string>(@"^NOT (\S+) -> (\S+)$", (a, b) => Wires[b] = new Not(a) {Signals = signals});
        matcher.AddRegex<string, string, string>(@"^(\S+) AND (\S+) -> (\S+)$", (a, b, c) => Wires[c] = new And(a, b) {Signals = signals});
        matcher.AddRegex<string, string, string>(@"^(\S+) OR (\S+) -> (\S+)$", (a, b, c) => Wires[c] = new Or(a, b) {Signals = signals});
        matcher.AddRegex<string, string, string>(@"^(\S+) RSHIFT (\S+) -> (\S+)$", (a, b, c) => Wires[c] = new RShift(a, b.AsInt()) {Signals = signals});
        matcher.AddRegex<string, string, string>(@"^(\S+) LSHIFT (\S+) -> (\S+)$", (a, b, c) => Wires[c] = new LShift(a, b.AsInt()) {Signals = signals});
        
        foreach (var s in Input)
        {
            matcher.Handle(s);
        }
    }
    
    public override int PartOne()
    {
        ReadWires();
        return GetSignal("a");
    }

    public override int PartTwo()
    {
        Part = 1;
        ReadWires();
        BOverride = GetSignal("a");
        Part = 2;
        Wires.Clear();
        ReadWires();
        return GetSignal("a");
    }

    public abstract class Signal
    {
        public Func<string, ushort> Signals { get; init; } = null!;

        public abstract ushort Data { get; }
    }

    public class Pass : Signal
    {
        private string _source;

        public Pass(string source) => _source = source;

        public override ushort Data => Signals(_source);
    }

    public class And : Signal
    {
        private string _left, _right;
        
        public And(string left, string right)
        {
            _left = left;
            _right = right;
        }

        public override ushort Data => (ushort) (Signals(_left) & Signals(_right));
    }
    
    public class Or : Signal
    {
        private string _left, _right;
        
        public Or(string left, string right)
        {
            _left = left;
            _right = right;
        }

        public override ushort Data => (ushort) (Signals(_left) | Signals(_right));
    }
    
    public class Not : Signal
    {
        private string _wire;

        public Not(string wire) => _wire = wire;

        public override ushort Data => (ushort) ~Signals(_wire);
    }
    
    public class LShift : Signal
    {
        private string _wire;
        private int _amount;

        public LShift(string wire, int amount)
        {
            _wire = wire;
            _amount = amount;
        }

        public override ushort Data => (ushort) (Signals(_wire) << _amount);
    }
    
    public class RShift : Signal
    {
        private string _wire;
        private int _amount;

        public RShift(string wire, int amount)
        {
            _wire = wire;
            _amount = amount;
        }

        public override ushort Data => (ushort) (Signals(_wire) >> _amount);
    }
}

public class Day7_2 : Puzzle<ushort>
{
    public Cpu<ushort> Cpu;

    public Day7_2()
    {
        // Part = 1;
        InputName = "Day7.txt";
    }

    public override ushort PartOne()
    {
        var builder = OpFormatBuilder<ushort>.Default(ushort.Parse);
        builder.Add("rd {->} ()", m => m);
        builder.Add("{NOT} rd _ ()", m => (ushort) ~m.Value);
        builder.Add("rd {AND} rd _ ()", (l, r) => (ushort) (l.Value & r.Value));
        builder.Add("rd {OR} rd _ ()", (l, r) => (ushort) (l.Value | r.Value));
        builder.Add("rd {RSHIFT} d _ ()", (l, r) => (ushort) (l.Value >> r.Value));
        builder.Add("rd {LSHIFT} d _ ()", (l, r) => (ushort) (l.Value << r.Value));
        
        Cpu = builder.ParseNodes(Input);
        return Cpu.Memory["a"];
    }

    public override ushort PartTwo()
    {
        var signalA = PartOne();
        Cpu.Memory.Reset();
        Cpu.Memory["b"] = signalA;
        return Cpu.Memory["a"];
    }
}