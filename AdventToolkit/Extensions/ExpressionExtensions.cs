using AdventToolkit.Utilities.Parsing;

namespace AdventToolkit.Extensions
{
    public static class ExpressionExtensions
    {
        public static SingleTypeExpression<int, TContext> WithOps<TContext>(this SingleTypeExpression<int, TContext> expr)
        {
            expr.AddBinary(new BinarySymbol("+", 1), (a, b) => a + b);
            expr.AddBinary(new BinarySymbol("-", 1), (a, b) => a - b);
            expr.AddBinary(new BinarySymbol("*", 2), (a, b) => a * b);
            expr.AddBinary(new BinarySymbol("/", 2), (a, b) => a / b);
            expr.AddUnary(new UnarySymbol("-"), i => -i);
            return expr;
        }
        
        public static SingleTypeExpression<long, TContext> WithOps<TContext>(this SingleTypeExpression<long, TContext> expr)
        {
            expr.AddBinary(new BinarySymbol("+", 1), (a, b) => a + b);
            expr.AddBinary(new BinarySymbol("-", 1), (a, b) => a - b);
            expr.AddBinary(new BinarySymbol("*", 2), (a, b) => a * b);
            expr.AddBinary(new BinarySymbol("/", 2), (a, b) => a / b);
            expr.AddUnary(new UnarySymbol("-"), i => -i);
            return expr;
        }
    }
}