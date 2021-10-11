using Microsoft.Z3;

namespace Z3Helper
{
    public readonly struct ZbExpr
    {
        public readonly BoolExpr Expr;

        public ZbExpr(BoolExpr expr) => Expr = expr;

        public static implicit operator ZbExpr(BoolExpr expr) => new(expr);

        public static implicit operator BoolExpr(ZbExpr expr) => expr.Expr;
    }
}