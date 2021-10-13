using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    // Wrapper for the root value of an expression
    public class Expression<T, TContext> : IContextValue<T, TContext>
    {
        private IContextValue<T, TContext> _root;

        internal Expression(IContextValue<T, TContext> root) => _root = root;

        public T GetValue(TContext context = default) => _root.GetValue(context);

        public static ExpressionReader<T, TContext> Reader() => new();
    }

    // Wrapper for when an expression doesn't need a context
    public class Expression<T> : Expression<T, int>
    {
        internal Expression(IContextValue<T, int> root) : base(root) { }
    }

    // Methods to help create an expression reader, such as adding definitions
    // for common operators (add, sub, multiply, divide, negate).
    public static class ExpressionHelpers
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
            reader.Functions.Add((name, args), new FunctionType<T, TC>(name, args, (p, _) => action(p)));
            return reader;
        }
    }

    public class ExpressionReader<T, TC>
    {
        private ExpressionNode<T, TC> _root;
        private Component _next = Component.UnaryOrValue;
        private ExpressionNode<T, TC> _current;

        public readonly List<BinaryOperatorType<T, TC>> BinaryOperators;
        public readonly List<UnaryOperatorType<T, TC>> UnaryOperators;
        public readonly Dictionary<(string name, int param), FunctionType<T, TC>> Functions;
        public Func<string, TokenType, ExpressionNode<T, TC>> ValueReader;

        public ExpressionReader()
        {
            BinaryOperators = new List<BinaryOperatorType<T, TC>>();
            UnaryOperators = new List<UnaryOperatorType<T, TC>>();
            Functions = new Dictionary<(string name, int param), FunctionType<T, TC>>();
        }

        private ExpressionReader(ExpressionReader<T, TC> other)
        {
            BinaryOperators = other.BinaryOperators;
            UnaryOperators = other.UnaryOperators;
            Functions = other.Functions;
            ValueReader = other.ValueReader;
        }

        // Find ending parenthesis
        private int FindEnd((string, TokenType)[] parts, int start)
        {
            var level = 0;
            for (var i = start; i < parts.Length; i++)
            {
                if (parts[i].Item1 == "(") level++;
                if (parts[i].Item1 == ")") level--;
                if (level == 0) return i;
            }
            return -1;
        }

        private UnaryOperatorType<T, TC> GetUnaryType(string symbol)
        {
            return UnaryOperators.Single(type => type.Symbol == symbol);
        }
        
        private BinaryOperatorType<T, TC> GetBinaryType(string symbol)
        {
            return BinaryOperators.Single(type => type.Symbol == symbol);
        }

        // Read a sub expression that's inside parenthesis.
        private bool ReadGroup((string, TokenType)[] sections, int index, out Group<T, TC> exp, out int end)
        {
            end = FindEnd(sections, index);
            exp = default;
            if (end < 0) return false;
            exp = new Group<T, TC>(new ExpressionReader<T, TC>(this).Read(sections[(index + 1)..end]));
            return true;
        }

        // If the reader is currently representing a valid expression.
        // This is false if an operator still needs an associated value.
        private bool ValidState()
        {
            if (_current is UnaryOperator<T, TC> {Child: { }}) return true;
            return _current is not Operator<T, TC>;
        }

        public T GetResult(string expr, TC context = default) => Read(expr).GetValue(context);

        public Expression<T, TC> Read(string expr) => Read(expr.Tokenize().ToArray());

        // Turn a sequence of tokens into an expression.
        // Read and construct an expression tree in one pass.
        public Expression<T, TC> Read((string token, TokenType type)[] sections)
        {
            _root = _current = null;
            _next = Component.UnaryOrValue;
            for (var i = 0; i < sections.Length; i++)
            {
                var (section, type) = sections[i];
                if (_next == Component.UnaryOrValue)
                {
                    if (type == TokenType.Symbol && section != "(")
                    {
                        var uOp = new UnaryOperator<T, TC>(GetUnaryType(section));
                        _current?.AddChild(uOp);
                        _current = uOp;
                    }
                    else
                    {
                        ExpressionNode<T, TC> value;
                        if (section == "(")
                        {
                            ReadGroup(sections, i, out var exp, out i);
                            value = exp;
                        }
                        else if (type == TokenType.Word && i + 1 < sections.Length && sections[i + 1].token == "(")
                        {
                            var end = FindEnd(sections, i + 1);
                            // TODO change this for nested function calls
                            var args = sections[(i + 2)..end].Split(",", TokenType.Symbol).Select(arg => new ExpressionReader<T, TC>(this).Read(arg)).ToArray();
                            if (Functions.TryGetValue((section, args.Length), out var funcType))
                            {
                                value = new Function<T, TC>(funcType) {Parameters = args};
                            }
                            else throw new Exception($"Function {section}[{args.Length}] does not exist.");
                            i = end;
                        }
                        else
                        {
                            value = ValueReader(section, type);
                        }
                        _current?.AddChild(value);
                        _current = value;
                        // Navigate to the top of a chain of unary operators
                        while (_current.Parent is UnaryOperator<T, TC>) _current = _current.Parent;
                        _next = Component.Binary;
                    }
                    _root ??= _current;
                }
                else if (_next == Component.Binary)
                {
                    if (type == TokenType.Symbol && section != "(")
                    {
                        var op = new BinaryOperator<T, TC>(GetBinaryType(section));
                        // Find the correct spot to insert the operator.
                        while (_current.Parent?.HigherPrecedenceThan(op) == true)
                        {
                            _current = _current.Parent;
                        }
                        _current.Parent?.Replace(_current, op);
                        op.AddChild(_current);
                        if (_current == _root) _root = op;
                        _current = op;
                        _next = Component.UnaryOrValue;
                    }
                    else
                    {
                        throw new Exception($"Expecting binary operator, found \"{section}\"");
                    }
                }
            }
            if (!ValidState()) throw new Exception("Incomplete expression.");
            return new Expression<T, TC>(_root);
        }

        private enum Component
        {
            UnaryOrValue,
            Binary
        }
    }

    public abstract class ExpressionNode<T, TC> : IContextValue<T, TC>
    {
        public ExpressionNode<T, TC> Parent;

        public abstract T GetValue(TC context);

        public abstract void AddChild(ExpressionNode<T, TC> node);

        public abstract void Replace(ExpressionNode<T, TC> a, ExpressionNode<T, TC> b);

        public bool HigherPrecedenceThan(BinaryOperator<T, TC> binaryOperator)
        {
            if (this is not BinaryOperator<T, TC> b) return false;
            return b.CompareTo(binaryOperator) > 0;
        }
    }

    public abstract class Leaf<T, TC> : ExpressionNode<T, TC>
    {
        public override void AddChild(ExpressionNode<T, TC> node) => throw new Exception("Leaf cannot have children.");

        public override void Replace(ExpressionNode<T, TC> a, ExpressionNode<T, TC> b) => throw new Exception("Leaf cannot have children.");
    }

    public class Constant<T, TC> : Leaf<T, TC>
    {
        public readonly T Value;

        public Constant(T value) => Value = value;

        public override T GetValue(TC context) => Value;
    }

    public class Variable<T, TC> : Leaf<T, TC>
    {
        private readonly Func<TC, T> _action;

        public Variable(Func<TC, T> action) => _action = action;

        public override T GetValue(TC context) => _action(context);
    }

    public class Group<T, TC> : Leaf<T, TC>
    {
        private readonly Expression<T, TC> _expression;

        public Group(Expression<T, TC> expression) => _expression = expression;

        public override T GetValue(TC context) => _expression.GetValue(context);
    }

    public abstract class Operator<T, TC> : ExpressionNode<T, TC>
    {
        public string Symbol { get; init; }

        public int Precedence { get; init; }

        public bool LeftAssociative { get; init; } = true;
    }

    public class BinaryOperator<T, TC> : Operator<T, TC>, IComparable<BinaryOperator<T, TC>>
    {
        public readonly Func<T, T, TC, T> Action;

        public ExpressionNode<T, TC> Left, Right;

        public BinaryOperator(BinaryOperatorType<T, TC> type) : this(type.Symbol, type.Action)
        {
            Precedence = type.Precedence;
            LeftAssociative = type.LeftAssociative;
        }
        
        public BinaryOperator(string symbol, Func<T, T, TC, T> action)
        {
            Action = action;
            Symbol = symbol;
        }

        public override T GetValue(TC context) => Action(Left.GetValue(context), Right.GetValue(context), context);

        public override void AddChild(ExpressionNode<T, TC> node)
        {
            if (Left == null) Left = node;
            else if (Right == null) Right = node;
            else throw new Exception("Node has reached maximum capacity.");
            node.Parent = this;
        }

        public override void Replace(ExpressionNode<T, TC> a, ExpressionNode<T, TC> b)
        {
            if (Left == a)
            {
                a.Parent = null;
                Left = b;
                b.Parent = this;
            }
            else if (Right == a)
            {
                a.Parent = null;
                Right = b;
                b.Parent = this;
            }
        }

        public int CompareTo(BinaryOperator<T, TC> other)
        {
            if (other.LeftAssociative)
            {
                return Precedence - other.Precedence + 1;
            }
            return Precedence - other.Precedence;
        }
    }

    public class UnaryOperator<T, TC> : Operator<T, TC>
    {
        public readonly Func<T, TC, T> Action;

        public ExpressionNode<T, TC> Child;

        public UnaryOperator(UnaryOperatorType<T, TC> type) : this(type.Symbol, type.Action) { }

        public UnaryOperator(string symbol, Func<T, TC, T> action)
        {
            Symbol = symbol;
            Action = action;
        }

        public override T GetValue(TC context) => Action(Child.GetValue(context), context);

        public override void AddChild(ExpressionNode<T, TC> node)
        {
            if (Child == null) Child = node;
            else throw new Exception("Node has reached maximum capacity.");
            node.Parent = this;
        }

        public override void Replace(ExpressionNode<T, TC> a, ExpressionNode<T, TC> b)
        {
            if (Child == a) Child = b;
        }
    }

    public class Function<T, TC> : Leaf<T, TC>
    {
        public readonly string Symbol;
        public readonly int ParameterCount;
        public readonly Func<T[], TC, T> Action;

        public Expression<T, TC>[] Parameters;
        
        public Function(FunctionType<T, TC> type)
        {
            Symbol = type.Symbol;
            ParameterCount = type.Parameters;
            Action = type.Action;
        }
        
        public override T GetValue(TC context)
        {
            return Action(Parameters.Select(exp => exp.GetValue(context)).ToArray(), context);
        }
    }

    public class BinaryOperatorType<T, TC>
    {
        public readonly string Symbol;
        public readonly Func<T, T, TC, T> Action;
        public readonly int Precedence;
        public readonly bool LeftAssociative;

        public BinaryOperatorType(string symbol, Func<T, T, TC, T> action, int precedence, bool leftAssociative = true)
        {
            Symbol = symbol;
            Action = action;
            Precedence = precedence;
            LeftAssociative = leftAssociative;
        }
    }

    public class UnaryOperatorType<T, TC>
    {
        public readonly string Symbol;
        public readonly Func<T, TC, T> Action;

        public UnaryOperatorType(string symbol, Func<T, TC, T> action)
        {
            Symbol = symbol;
            Action = action;
        }
    }

    public class FunctionType<T, TC>
    {
        public readonly string Symbol;
        public readonly int Parameters;
        public readonly Func<T[], TC, T> Action;

        public FunctionType(string symbol, int parameters, Func<T[], TC, T> action)
        {
            Symbol = symbol;
            Parameters = parameters;
            Action = action;
        }
    }
}