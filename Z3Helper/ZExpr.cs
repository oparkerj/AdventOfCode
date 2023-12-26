using Microsoft.Z3;

#pragma warning disable CS0660, CS0661

namespace Z3Helper;

/// <summary>
/// Wrapper for an expr in Z3
/// </summary>
public readonly struct ZExpr
{
    public readonly ArithExpr Expr;

    public ZExpr(ArithExpr expr) => Expr = expr;

    public static implicit operator ZExpr(ArithExpr expr) => new(expr);

    public static implicit operator ArithExpr(ZExpr expr) => expr.Expr;

    public static implicit operator ZExpr(int i) => i.ZInt();
    
    public static implicit operator ZExpr(long i) => i.ZInt();

    public static implicit operator ZExpr(string s) => s.IntConst();

    public static ZbExpr operator ==(ZExpr left, ZExpr right) => left.Eq(right);

    public static ZbExpr operator !=(ZExpr left, ZExpr right) => left.Neq(right);

    public static ZbExpr operator >=(ZExpr left, ZExpr right) => left.Ge(right);

    public static ZbExpr operator <=(ZExpr left, ZExpr right) => left.Le(right);

    public static ZbExpr operator >(ZExpr left, ZExpr right) => left.Gt(right);
    
    public static ZbExpr operator <(ZExpr left, ZExpr right) => left.Lt(right);

    public static ZExpr operator +(ZExpr left, ZExpr right) => left.Add(right);

    public static ZExpr operator -(ZExpr left, ZExpr right) => left.Sub(right);

    public static ZExpr operator *(ZExpr left, ZExpr right) => left.Mul(right);

    public static ZExpr operator /(ZExpr left, ZExpr right) => left.Div(right);
}