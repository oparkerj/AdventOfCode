using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;

namespace Z3Helper;

public static class Zzz
{
    public static readonly Lazy<Context> Z3Context = new(() => new Context());

    public static Context Context => Z3Context.Value;

    public static Optimize Optimize() => Context.MkOptimize();

    public static Solver Solver() => Context.MkSolver();

    public static IEnumerable<ArithExpr> Z3(this IEnumerable<ZExpr> exprs)
    {
        return exprs.Select(expr => (ArithExpr) expr);
    }
    
    public static IEnumerable<BoolExpr> Z3(this IEnumerable<ZbExpr> exprs)
    {
        return exprs.Select(expr => (BoolExpr) expr);
    }

    public static ZExpr Int(this int i) => Context.MkInt(i);
    
    public static ZExpr Int(this long i) => Context.MkInt(i);

    public static ZExpr IntConst(this string s) => Context.MkIntConst(s);
    
    public static ZExpr RealConst(this string s) => Context.MkRealConst(s);

    public static ZExpr Variable(this string s) => s.IntConst();
    
    public static ZExpr RealVariable(this string s) => s.RealConst();

    public static ZExpr Condition(this ZbExpr expr, ZExpr left, ZExpr right)
    {
        return (ArithExpr) Context.MkITE(expr, left, right);
    }

    public static ZExpr Condition(this BoolExpr expr, ZExpr left, ZExpr right)
    {
        return Condition((ZbExpr) expr, left, right);
    }

    public static ZExpr Abs(this ZExpr expr)
    {
        return (expr >= 0).Condition(expr, 0 - expr);
    }

    public static ZExpr Sum(this IEnumerable<ZExpr> exprs) => AddAll(exprs.ToArray());

    public static ZExpr Product(this IEnumerable<ZExpr> exprs) => MulAll(exprs.ToArray());

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

    public static ZExpr MulAll(params ZExpr[] exprs)
    {
        return Context.MkMul(exprs.Z3());
    }

    public static ZExpr Mul(this ZExpr left, ZExpr right)
    {
        return MulAll(left, right);
    }

    public static ZExpr Mul(this ArithExpr left, ZExpr right)
    {
        return Mul((ZExpr) left, right);
    }

    public static ZExpr Div(this ZExpr left, ZExpr right)
    {
        return Context.MkDiv(left, right);
    }

    public static ZExpr Div(this ArithExpr left, ZExpr right)
    {
        return Div((ZExpr) left, right);
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

    public static ZbExpr Gt(this ZExpr left, ZExpr right)
    {
        return Context.MkGt(left, right);
    }

    public static ZbExpr Gt(this ArithExpr left, ZExpr right)
    {
        return Gt((ZExpr) left, right);
    }
        
    public static ZbExpr Le(this ZExpr left, ZExpr right)
    {
        return Context.MkLe(left, right);
    }

    public static ZbExpr Le(this ArithExpr left, ZExpr right)
    {
        return Le((ZExpr) left, right);
    }
    
    public static ZbExpr Lt(this ZExpr left, ZExpr right)
    {
        return Context.MkLt(left, right);
    }

    public static ZbExpr Lt(this ArithExpr left, ZExpr right)
    {
        return Lt((ZExpr) left, right);
    }

    public static ZExpr Min(this ZExpr expr, ZExpr min)
    {
        return (expr > min).Condition(expr, min);
    }

    public static ZExpr Min(this ArithExpr expr, ZExpr min)
    {
        return Min((ZExpr) expr, min);
    }
    
    public static ZExpr Max(this ZExpr expr, ZExpr max)
    {
        return (expr < max).Condition(expr, max);
    }

    public static ZExpr Max(this ArithExpr expr, ZExpr max)
    {
        return Max((ZExpr) expr, max);
    }
}