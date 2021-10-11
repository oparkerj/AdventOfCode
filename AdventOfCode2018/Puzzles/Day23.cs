using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using Microsoft.Z3;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2018.Puzzles
{
    public class Day23 : Puzzle
    {
        private Context _context;

        public Day23()
        {
            Part = 2;
        }

        public IEnumerable<Nanobot> ReadBots()
        {
            return Input.Extract<Nanobot>(@"pos=(<.+?>), r=(\d+)");
        }

        public override void PartOne()
        {
            var bots = ReadBots().ToArray(Input.Length);
            var (position, radius) = bots.MaxBy(bot => bot.Radius).First();
            var count = bots.Count(bot => bot.Position.MDist(position) <= radius);
            WriteLn(count);
        }

        public override void PartTwo()
        {
            var bots = ReadBots().ToArray(Input.Length);

            using var c = _context = new Context();

            var x = Const("x");
            var y = Const("y");
            var z = Const("z");
            var inRange = bots.Select(bot => IfThenElse(
                LessThanEqual(
                    Add(Abs(Sub(x, Int(bot.Position.X))),
                        Abs(Sub(y, Int(bot.Position.Y))),
                        Abs(Sub(z, Int(bot.Position.Z)))),
                    Int(bot.Radius)),
                Int(1),
                Int(0))).ToArray();
            
            var sum = Const("sum");
            var dist = Const("dist");
            
            var o = c.MkOptimize();
            // sum = Number in range at x, y, z
            o.Add(Equals(sum, Add(inRange)));
            // dist = Manhattan distance to 0, 0, 0.
            o.Add(Equals(dist, Add(Abs(x), Abs(y), Abs(z))));

            var maxSum = o.MkMaximize(sum);
            var minDist = o.MkMinimize(dist);
            
            WriteLn(o.Check());
            WriteLn(maxSum.Value);
            WriteLn(minDist.Value);
        }

        private IntExpr Const(string name)
        {
            return _context.MkIntConst(name);
        }

        private ArithExpr Int(int i)
        {
            return _context.MkInt(i);
        }

        private ArithExpr Add(params ArithExpr[] exprs)
        {
            return _context.MkAdd(exprs);
        }
        
        private ArithExpr Sub(params ArithExpr[] exprs)
        {
            return _context.MkSub(exprs);
        }

        private ArithExpr Abs(ArithExpr e)
        {
            return IfThenElse(GreaterThanEqual(e, Int(0)),
                e,
                Sub(Int(0), e));
        }

        private BoolExpr Equals(Expr left, Expr right)
        {
            return _context.MkEq(left, right);
        }

        private ArithExpr IfThenElse(BoolExpr cond, Expr t, Expr f)
        {
            return (ArithExpr) _context.MkITE(cond, t, f);
        }

        private BoolExpr GreaterThanEqual(ArithExpr left, ArithExpr right)
        {
            return _context.MkGe(left, right);
        }
        
        private BoolExpr LessThanEqual(ArithExpr left, ArithExpr right)
        {
            return _context.MkLe(left, right);
        }

        public record Nanobot(Pos3D Position, int Radius);
    }
}