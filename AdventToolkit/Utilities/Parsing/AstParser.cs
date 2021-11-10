using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities.Parsing
{
    public class AstParser<T>
    {
        public readonly List<INodeConverter<T>> Converters = new();

        public AstParser<T> Add(INodeConverter<T> converter)
        {
            Converters.Add(converter);
            return this;
        }

        public bool TryParse(AstNode node, out T result)
        {
            foreach (var converter in Converters)
            {
                if (converter.TryParse(this, node, out result)) return true;
            }
            result = default;
            return false;
        }

        public T Parse(AstNode node)
        {
            return TryParse(node, out var result) ? result : default;
        }
    }

    public interface INodeConverter<T>
    {
        bool TryParse(AstParser<T> parser, AstNode node, out T result);
    }

    public class ValueParser<T> : INodeConverter<T>
    {
        public readonly ValueConverter Converter;

        public ValueParser(ValueConverter converter) => Converter = converter;

        public ValueParser(Func<Token, T> converter)
        {
            Converter = (Token value, out T result) =>
            {
                result = converter(value);
                return true;
            };
        }

        public ValueParser(TokenType type, TokenTypeConverter converter)
        {
            Converter = (Token value, out T result) =>
            {
                result = default;
                if (value.Type != type) return false;
                return converter(value.Content, out result);
            };
        }

        public ValueParser(TokenType type, Func<string, T> converter)
        {
            Converter = (Token value, out T result) =>
            {
                result = default;
                if (value.Type != type) return false;
                result = converter(value.Content);
                return true;
            };
        }

        public ValueParser(TokenTypeConverter converter)
        {
            Converter = (Token value, out T result) => converter(value.Content, out result);
        }

        public ValueParser(Func<string, T> converter)
        {
            Converter = (Token value, out T result) =>
            {
                result = converter(value.Content);
                return true;
            };
        }

        public delegate bool ValueConverter(Token value, out T result);

        public delegate bool TokenTypeConverter(string value, out T result);

        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not AstValue value) return false;
            return Converter(value.Value, out result);
        }
    }

    public class GroupUnwrapParser<T> : INodeConverter<T>
    {
        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not AstGroup group) return false;
            return parser.TryParse(group.Content, out result);
        }
    }

    public class UnaryOperatorParser<T> : INodeConverter<T>
    {
        public readonly UnaryOperatorConverter Converter;

        public UnaryOperatorParser(UnaryOperatorConverter converter) => Converter = converter;

        public UnaryOperatorParser(Func<string, T, T> converter)
        {
            Converter = (string op, T value, out T result) =>
            {
                result = converter(op, value);
                return true;
            };
        }

        public delegate bool UnaryOperatorConverter(string op, T value, out T result);

        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not UnaryOperator op) return false;
            if (!parser.TryParse(op.Value, out var value)) return false;
            return Converter(op.Symbol.Symbol, value, out result);
        }
    }

    public class BinaryOperatorParser<T> : INodeConverter<T>
    {
        public readonly BinaryOperatorConverter Converter;

        public BinaryOperatorParser(BinaryOperatorConverter converter) => Converter = converter;

        public BinaryOperatorParser(Func<T, string, T, T> converter)
        {
            Converter = (T left, string op, T right, out T result) =>
            {
                result = converter(left, op, right);
                return true;
            };
        }

        public delegate bool BinaryOperatorConverter(T left, string op, T right, out T result);
        
        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not BinaryOperator op) return false;
            if (!parser.TryParse(op.Left, out var left)) return false;
            if (!parser.TryParse(op.Right, out var right)) return false;
            return Converter(left, op.Symbol.Symbol, right, out result);
        }
    }

    public class TupleParser<T> : INodeConverter<T>
    {
        public readonly TupleConverter Converter;

        public TupleParser(TupleConverter converter) => Converter = converter;

        public TupleParser(GroupSymbol symbol, TupleGroupConverter converter)
        {
            Converter = (GroupSymbol groupSymbol, IList<T> args, out T result) =>
            {
                result = default;
                if (groupSymbol != symbol) return false;
                return converter(args, out result);
            };
        }
        
        public TupleParser(string left, string right, TupleGroupConverter converter) : this(new GroupSymbol(left, right), converter) { }

        public TupleParser(TupleGroupConverter converter)
        {
            Converter = (GroupSymbol _, IList<T> args, out T result) => converter(args, out result);
        }

        public delegate bool TupleConverter(GroupSymbol symbol, IList<T> args, out T result);

        public delegate bool TupleGroupConverter(IList<T> args, out T result);
        
        public static bool TryParseTuple(AstParser<T> parser, AstNode node, out IList<T> result)
        {
            result = default;
            if (node is not AstGroup group) return false;
            var content = group.Content;
            if (content is null)
            {
                result = Array.Empty<T>();
                return true;
            }
            if (content is AstSequence {Split: ","} sequence)
            {
                var temp = new T[sequence.Components.Count];
                if (sequence.Components.Where((component, i) => !parser.TryParse(component, out temp[i])).Any())
                {
                    return false;
                }
                result = temp;
                return true;
            }
            if (!parser.TryParse(content, out var value)) return false;
            result = new[] {value};
            return true;
        }

        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not AstGroup group) return false;
            if (!TryParseTuple(parser, node, out var args)) return false;
            return Converter(group.Symbol, args, out result);
        }
    }

    public class FunctionParser<T> : INodeConverter<T>
    {
        public readonly FunctionConverter Converter;

        public FunctionParser(FunctionConverter converter) => Converter = converter;

        public delegate bool FunctionConverter(string name, IList<T> args, out T result);

        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is not AstSequence sequence) return false;
            var components = sequence.Components;
            if (components.Count != 2) return false;
            if (components[0] is not AstValue name) return false;
            if (components[1] is not AstGroup args) return false;
            if (!TupleParser<T>.TryParseTuple(parser, args, out var argValues)) return false;
            return Converter(name.Value.Content, argValues, out result);
        }
    }

    public class SequenceParser<T> : INodeConverter<T>
    {
        public readonly RawConverter Converter;

        public SequenceParser(RawConverter converter) => Converter = converter;

        public SequenceParser(SequenceConverter converter)
        {
            Converter = (AstParser<T> parser, IList<AstNode> nodes, out T result) =>
            {
                result = default;
                var parts = new T[nodes.Count];
                if (nodes.Select((n, i) => !parser.TryParse(n, out parts[i])).Any())
                {
                    return false;
                }
                return converter(parts, out result);
            };
        }

        public delegate bool SequenceConverter(IList<T> args, out T result);

        public delegate bool RawConverter(AstParser<T> parser, IList<AstNode> nodes, out T result);

        public bool TryParse(AstParser<T> parser, AstNode node, out T result)
        {
            result = default;
            if (node is AstValue)
            {
                var data = new[] {node};
                return Converter(parser, data, out result);
            }
            if (node is not AstSequence sequence) return false;
            return Converter(parser, sequence.Components, out result);
        }
    }
}