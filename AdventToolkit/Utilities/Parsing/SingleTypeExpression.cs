using System;
using System.Collections.Generic;

namespace AdventToolkit.Utilities.Parsing
{
    public class SingleTypeExpression<T, TContext>
    {
        protected readonly Func<string, TContext, T> ValueReader;
        protected readonly Dictionary<string, Func<T, T, T>> BinaryOperators = new();
        protected readonly Dictionary<string, Func<T, T>> UnaryOperators = new();

        private Lazy<AstParser<IContextValue<T, TContext>>> _parser;
        protected AstReader _reader = new();

        public bool UpdateReader { get; set; } = true;

        private SingleTypeExpression()
        {
            _parser = new Lazy<AstParser<IContextValue<T, TContext>>>(CreateParser);
            _reader.GroupSymbols["("] = new GroupSymbol("(", ")");
        }

        public SingleTypeExpression(Func<string, TContext, T> valueReader) : this()
        {
            ValueReader = valueReader;
        }

        private AstParser<IContextValue<T, TContext>> CreateParser()
        {
            var parser = new AstParser<IContextValue<T, TContext>>();
            parser.Add(new ValueParser<IContextValue<T, TContext>>(s => new ExpValue<T, TContext>(s, ValueReader)));
            parser.Add(new GroupUnwrapParser<IContextValue<T, TContext>>());
            parser.Add(new BinaryOperatorParser<IContextValue<T, TContext>>((left, s, right) => new ExpBinary<T, TContext>(left, right, BinaryOperators[s])));
            parser.Add(new UnaryOperatorParser<IContextValue<T, TContext>>((s, value) => new ExpUnary<T, TContext>(value, UnaryOperators[s])));
            return parser;
        }

        public void AddBinary(BinarySymbol symbol, Func<T, T, T> operation)
        {
            _reader.BinarySymbols[symbol.Symbol] = symbol;
            BinaryOperators[symbol.Symbol] = operation;
        }

        public void RemoveBinary(string s)
        {
            _reader.BinarySymbols.Remove(s);
            BinaryOperators.Remove(s);
        }

        public void AddUnary(UnarySymbol symbol, Func<T, T> operation)
        {
            _reader.UnarySymbols[symbol.Symbol] = symbol;
            UnaryOperators[symbol.Symbol] = operation;
        }

        public void RemoveUnary(string s)
        {
            _reader.UnarySymbols.Remove(s);
            UnaryOperators.Remove(s);
        }

        public virtual IContextValue<T, TContext> Parse(string s)
        {
            if (_parser.Value.TryParse(_reader.Read(s), out var result)) return result;
            throw new Exception("Could not parse input.");
        }

        public T Eval(string s, TContext context) => Parse(s).GetValue(context);
    }

    // Optimized parsing when there is no context.
    // Normally the parse tree values are wrapped with the extra info needed to
    // compute the expression, but with no context, the final value can be
    // directly computed. Calling Parse on this object will return a wrapper
    // that contains the final computed value.
    public class SingleTypeExpression<T> : SingleTypeExpression<T, NoContext>
    {
        private AstParser<T> _parser;

        public SingleTypeExpression(Func<string, T> valueReader) : base((s, _) => valueReader(s))
        {
            _parser = CreateParser();
        }

        private AstParser<T> CreateParser()
        {
            var parser = new AstParser<T>();
            parser.Add(new ValueParser<T>(s => ValueReader(s, default)));
            parser.Add(new GroupUnwrapParser<T>());
            parser.Add(new BinaryOperatorParser<T>((a, s, b) => BinaryOperators[s](a, b)));
            parser.Add(new UnaryOperatorParser<T>((s, v) => UnaryOperators[s](v)));
            return parser;
        }

        public override IContextValue<T, NoContext> Parse(string s)
        {
            return new ExpDirectValue<T, NoContext>(Eval(s));
        }

        public T Eval(string s)
        {
            if (_parser.TryParse(_reader.Read(s), out var result)) return result;
            throw new Exception("Could not parse input.");
        }
    }

    public record ExpDirectValue<T, TContext>(T Value) : IContextValue<T, TContext>
    {
        public T GetValue(TContext context) => Value;
    }

    public record ExpValue<T, TContext>(string Value, Func<string, TContext, T> Converter) : IContextValue<T, TContext>
    {
        public T GetValue(TContext context) => Converter(Value, context);
    }

    public record ExpBinary<T, TContext>(IContextValue<T, TContext> Left, IContextValue<T, TContext> Right, Func<T, T, T> Operation) : IContextValue<T, TContext>
    {
        public T GetValue(TContext context) => Operation(Left.GetValue(context), Right.GetValue(context));
    }

    public record ExpUnary<T, TContext>(IContextValue<T, TContext> Value, Func<T, T> Operation) : IContextValue<T, TContext>
    {
        public T GetValue(TContext context) => Operation(Value.GetValue(context));
    }

}