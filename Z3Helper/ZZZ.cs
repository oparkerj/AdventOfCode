using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;

namespace Z3Helper
{
    public static class Zzz
    {
        public static readonly Lazy<Context> Z3Context = new(() => new Context());

        public static Context Context => Z3Context.Value;

        public static Optimize Optimize() => Context.MkOptimize();

        public static IEnumerable<ArithExpr> Z3(this IEnumerable<ZExpr> exprs)
        {
            return exprs.Select(expr => (ArithExpr) expr);
        }

        public static ZExpr Int(this int i) => Context.MkInt(i);

        public static ZExpr IntConst(this string s) => Context.MkIntConst(s);

        public static ZExpr Condition(this BoolExpr expr, ZExpr left, ZExpr right)
        {
            return (ArithExpr) Context.MkITE(expr, left, right);
        }

        public static ZExpr Condition(this ZbExpr expr, ZExpr left, ZExpr right)
        {
            return Condition((BoolExpr) expr, left, right);
        }

        public static ZExpr Abs(this ZExpr expr)
        {
            return (expr >= 0).Condition(expr, 0 - expr);
        }

        public static ZExpr Sum(this IEnumerable<ZExpr> exprs) => AddAll(exprs.ToArray());

        public static ZExpr AddAll(params ZExpr[] exprs)
        {
            return Context.MkAdd(exprs.Z3());
        }

        public static ZExpr Add(this ZExpr left, ZExpr right)
        {
            return AddAll(left, right);
        }

        public static ZExpr Add(this ArithExpr left, ZExpr right)
        {
            return Add((ZExpr) left, right);
        }

        public static ZExpr SubAll(params ZExpr[] exprs)
        {
            return Context.MkSub(exprs.Z3().ToArray());
        }

        public static ZExpr Sub(this ZExpr left, ZExpr right)
        {
            return SubAll(left, right);
        }

        public static ZExpr Sub(this ArithExpr left, ZExpr right)
        {
            return Sub((ZExpr) left, right);
        }

        public static ZbExpr Eq(this ZExpr left, ZExpr right)
        {
            return Context.MkEq(left, right);
        }

        public static ZbExpr Eq(this ArithExpr left, ZExpr right)
        {
            return Eq((ZExpr) left, right);
        }

        public static ZbExpr Neq(this ZExpr left, ZExpr right)
        {
            return Context.MkNot(left == right);
        }

        public static ZbExpr Neq(this ArithExpr left, ZExpr right)
        {
            return Neq((ZExpr) left, right);
        }

        public static ZbExpr Ge(this ZExpr left, ZExpr right)
        {
            return Context.MkGe(left, right);
        }

        public static ZbExpr Ge(this ArithExpr left, ZExpr right)
        {
            return Ge((ZExpr) left, right);
        }
        
        public static ZbExpr Le(this ZExpr left, ZExpr right)
        {
            return Context.MkLe(left, right);
        }

        public static ZbExpr Le(this ArithExpr left, ZExpr right)
        {
            return Le((ZExpr) left, right);
        }
    }
}