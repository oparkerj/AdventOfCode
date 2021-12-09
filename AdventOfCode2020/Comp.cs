using System.Linq;

namespace AdventOfCode2020;

public class Comp
{
    private Inst[] _source;

    public Comp(string[] source)
    {
        _source = source.Select(Convert).ToArray();
    }

    public int Length => _source.Length;

    public int Accumulator { get; private set; }

    public int Pointer { get; private set; } = 0;

    public bool Done => Pointer == _source.Length;

    public Inst this[int p]
    {
        get => _source[p];
        set => _source[p] = value;
    }

    private Inst Convert(string inst)
    {
        var i = inst.IndexOf(' ');
        var op = inst[..i];
        var arg = int.Parse(inst[(i + 1)..]);
        return op switch
        {
            "acc" => new Inst(Op.Acc, arg, 1),
            "jmp" => new Inst(Op.Jmp, 0, arg),
            _ => new Inst(Op.Nop, 0, 1)
        };
    }

    public void Reset()
    {
        Pointer = 0;
        Accumulator = 0;
    }

    public void Step()
    {
        var inst = _source[Pointer];
        Accumulator += inst.Acc;
        Pointer += inst.Offset;
    }

    public enum Op
    {
        Acc,
        Jmp,
        Nop
    }

    public class Inst
    {
        public Op Op { get; set; }
        public int Acc { get; set; }
        public int Offset { get; set; }

        public Inst(Op op, int acc, int offset)
        {
            Op = op;
            Acc = acc;
            Offset = offset;
        }
    }
}