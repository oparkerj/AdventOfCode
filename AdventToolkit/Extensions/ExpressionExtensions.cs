using System;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    // Methods to help create an expression reader, such as adding definitions
    // for common operators (add, sub, multiply, divide, negate).
    public static class ExpressionExtensions
    {
        public static ExpressionReader<int, T> WithOps<T>(this ExpressionReader<int, T> reader)
        {
            reader.BinaryOperators.Add(new BinaryOperatorType<int, T>("+", (a, b, _) => a + b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<int, T>("-", (a, b, _) => a - b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<int, T>("*", (a, b, _) => a * b, 2));
            reader.BinaryOperators.Add(new BinaryOperatorType<int, T>("/", (a, b, _) => a / b, 2));
            reader.UnaryOperators.Add(new UnaryOperatorType<int, T>("-", (i, _) => -i));
            return reader;
        }

        public static ExpressionReader<long, T> WithOps<T>(this ExpressionReader<long, T> reader)
        {
            reader.BinaryOperators.Add(new BinaryOperatorType<long, T>("+", (a, b, _) => a + b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<long, T>("-", (a, b, _) => a - b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<long, T>("*", (a, b, _) => a * b, 2));
            reader.BinaryOperators.Add(new BinaryOperatorType<long, T>("/", (a, b, _) => a / b, 2));
            reader.UnaryOperators.Add(new UnaryOperatorType<long, T>("-", (i, _) => -i));
            return reader;
        }

        public static ExpressionReader<double, T> WithOps<T>(this ExpressionReader<double, T> reader)
        {
            reader.BinaryOperators.Add(new BinaryOperatorType<double, T>("+", (a, b, _) => a + b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<double, T>("-", (a, b, _) => a - b, 1));
            reader.BinaryOperators.Add(new BinaryOperatorType<double, T>("*", (a, b, _) => a * b, 2));
            reader.BinaryOperators.Add(new BinaryOperatorType<double, T>("/", (a, b, _) => a / b, 2));
            reader.UnaryOperators.Add(new UnaryOperatorType<double, T>("-", (i, _) => -i));
            return reader;
        }

        public static ExpressionReader<T, TC> SetReader<T, TC>(this ExpressionReader<T, TC> reader, Func<string, TokenType, ExpressionNode<T, TC>> func)
        {
            reader.ValueReader = func;
            return reader;
        }

        // Set up the reader for all values in the expression being constants,
        // using the provided function to parse them from strings.
        public static ExpressionReader<T, TC> ForConstants<T, TC>(this ExpressionReader<T, TC> reader, Func<string, T> func)
        {
            return reader.SetReader((s, _) => new Constant<T, TC>(func(s)));
        }

        public static ExpressionReader<int, TC> ForConstants<TC>(this ExpressionReader<int, TC> reader)
        {
            return reader.ForConstants(int.Parse);
        }

        public static ExpressionReader<long, TC> ForConstants<TC>(this ExpressionReader<long, TC> reader)
        {
            return reader.ForConstants(long.Parse);
        }

        // Set up the reader for values to be read as variables, using the provided
        // function to get the value of a variable from the context.
        public static ExpressionReader<T, TC> ForVariables<T, TC>(this ExpressionReader<T, TC> reader, Func<string, TC, T> func)
        {
            return reader.SetReader((s, _) => new Variable<T, TC>(c => func(s, c)));
        }

        public delegate bool FallbackParser<T, TC>(string s, TokenType type, out Func<string, TC, T> func);
        
        // Set up the reader to try and read a value as a variable, but fallback to the
        // original reader on failure.
        public static ExpressionReader<T, TC> AndVariables<T, TC>(this ExpressionReader<T, TC> reader, FallbackParser<T, TC> parser)
        {
            var fallback = reader.ValueReader;
            return reader.SetReader((s, type) => parser(s, type, out var func) ? new Variable<T, TC>(c => func(s, c)) : fallback(s, type));
        }

        public static ExpressionReader<T, TC> AddBinaryOp<T, TC>(this ExpressionReader<T, TC> reader, string symbol, Func<T, T, TC, T> action, int precedence, bool leftAssociative = true)
        {
            reader.BinaryOperators.Add(new BinaryOperatorType<T, TC>(symbol, action, precedence, leftAssociative) );
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddBinaryOp<T, TC>(this ExpressionReader<T, TC> reader, string symbol, Func<T, T, T> action, int precedence, bool leftAssociative = true)
        {
            return reader.AddBinaryOp(symbol, (a, b, _) => action(a, b), precedence, leftAssociative);
        }
        
        public static ExpressionReader<T, TC> AddUnaryOp<T, TC>(this ExpressionReader<T, TC> reader, string symbol, Func<T, TC, T> action, int precedence)
        {
            reader.UnaryOperators.Add(new UnaryOperatorType<T, TC>(symbol, action));
            return reader;
        }
        public static ExpressionReader<T, TC> AddUnaryOp<T, TC>(this ExpressionReader<T, TC> reader, string symbol, Func<T, T> action, int precedence)
        {
            return reader.AddUnaryOp(symbol, (v, _) => action(v), precedence);
        }

        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, int args, Func<T[], TC, T> action)
        {
            reader.Functions.Add((name, args), new FunctionType<T, TC>(name, args, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, int args, Func<T[], T> action)
        {
            reader.Functions.Add((name, args), new FunctionType<T, TC>(name, args, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<T> action)
        {
            reader.Functions.Add((name, 0), new FunctionType<T, TC>(name, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<TC, T> action)
        {
            reader.Functions.Add((name, 0), new FunctionType<T, TC>(name, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<T, T> action)
        {
            reader.Functions.Add((name, 1), new FunctionType<T, TC>(name, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<TC, T, T> action)
        {
            reader.Functions.Add((name, 1), new FunctionType<T, TC>(name, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<T, T, T> action)
        {
            reader.Functions.Add((name, 2), new FunctionType<T, TC>(name, action));
            return reader;
        }
        
        public static ExpressionReader<T, TC> AddFunction<T, TC>(this ExpressionReader<T, TC> reader, string name, Func<TC, T, T, T> action)
        {
            reader.Functions.Add((name, 2), new FunctionType<T, TC>(name, action));
            return reader;
        }
    }
}