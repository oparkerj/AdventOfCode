using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Computer.Builders.Opcode;
using Microsoft.Z3;
using Z3Helper;

namespace AdventOfCode2022.Puzzles;

public class Day21 : Puzzle<long>
{
    public override long PartOne()
    {
        var builder = OpFormatBuilder<long>.Default(long.Parse);
        builder.Add("() d", v => v);
        builder.Add("() r {+} r", (a, b) => a + b);
        builder.Add("() r {-} r", (a, b) => a - b);
        builder.Add("() r {*} r", (a, b) => a * b);
        builder.Add("() r {/} r", (a, b) => a / b);

        var input = Input.Select(s => s.Replace(":", ""));
        var nodeCpu = builder.ParseNodes(input);
        return nodeCpu.Memory["root"];
    }

    public override long PartTwo()
    {
        var vars = Input.Select(s => s.Before(':'))
            .ToDictionary(s => s, s => s.IntConst());

        ZExpr rootLeft = default;
        ZExpr rootRight = default;
        
        var parts = new Dictionary<string, ZExpr>();
        foreach (var s in Input)
        {
            var name = s.Before(':');
            vars[name] = name.IntConst();
            if (s.Count(' ') == 1)
            {
                parts[name] = s.After(' ').AsInt().Int();
            }
            else
            {
                var part = s.Split(' ');
                var left = vars[part[1]];
                var right = vars[part[3]];
                var op = part[2];

                if (name == "root")
                {
                    rootLeft = left;
                    rootRight = right;
                    continue;
                }
                
                parts[name] = op switch
                {
                    "+" => left + right,
                    "-" => left - right,
                    "*" => left * right,
                    "/" => left / right,
                    _ => throw new Exception()
                };
            }
        }
        parts.Remove("humn");

        var solver = Zzz.Context.MkSolver();
        foreach (var (name, value) in parts)
        {
            solver.Assert(vars[name] == value);
        }
        solver.Assert(rootLeft == rootRight);

        if (solver.Check() == Status.SATISFIABLE)
        {
            return solver.Model.ConstInterp(vars["humn"])
                .ToString()
                .AsLong();
        }
        WriteLn(solver.Proof);
        return -1;
    }
}