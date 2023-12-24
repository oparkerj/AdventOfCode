using Microsoft.Z3;

namespace Z3Helper;

/// <summary>
/// Wrapper for a bool expr in Z3.
/// </summary>
public readonly struct ZbExpr
{
    public readonly BoolExpr Expr;

    public ZbExpr(BoolExpr expr) => Expr = expr;

    public static implicit operator ZbExpr(BoolExpr expr) => new(expr);

    public static implicit operator BoolExpr(ZbExpr expr) => expr.Expr;
}