using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities
{
    // Parse a string into an AST tree.
    // Parses basic structures including binary operators, unary operators,
    // and delimited groups. If the start and ending symbols of a group are
    // the same, some assumptions are made about which side is the start or end.
    // For example, with the Group |...|, seeing it after a binary operator will
    // be treated as the opening side; Likewise seeing it where a binary operator
    // is normally expected will be treated as the end of the group.
    public class AstReader
    {
        public readonly Dictionary<string, BinarySymbol> BinarySymbols;
        public readonly Dictionary<string, UnarySymbol> UnarySymbols;
        public readonly Dictionary<string, GroupSymbol> GroupSymbols;
        public string SequenceSplit;

        // private readonly GroupSymbol _currentGroup;
        
        public AstReader()
        {
            BinarySymbols = new Dictionary<string, BinarySymbol>();
            UnarySymbols = new Dictionary<string, UnarySymbol>();
            GroupSymbols = new Dictionary<string, GroupSymbol>();
        }

        private AstReader(AstReader parent, GroupSymbol group)
        {
            BinarySymbols = parent.BinarySymbols;
            UnarySymbols = parent.UnarySymbols;
            GroupSymbols = parent.GroupSymbols;
            // _currentGroup = group;
        }

        private Dictionary<int, int> FindBounds(Token[] tokens)
        {
            var expect = new Stack<string>();
            var bounds = new Dictionary<int, int>();
            var seen = new List<(string Open, int Counter, int Level)>();
            var opens = new HashSet<string>(GroupSymbols.Values.Select(symbol => symbol.Left));
            var close = new HashSet<string>(GroupSymbols.Values.Select(symbol => symbol.Right));
            var depth = 0;
            for (var i = 0; i < tokens.Length; i++)
            {
                var s = tokens[i].Content;
                if (opens.Contains(s))
                {
                    var nextDepth = GroupSymbols[s].Right == s ? depth : depth + 1;
                    seen.Add((s, depth + 1, nextDepth));
                }
            }
        }

        private AstNode Read(Token[] tokens, int start, out int end, Dictionary<int, int> bounds)
        {
            var unary = UnarySymbols;
            var binary = BinarySymbols;
            var groups = GroupSymbols;
            var split = SequenceSplit;
            AstNode root = null;
            AstNode current = null;

            void Insert(AstNode node)
            {
                if (current == null)
                {
                    current = node;
                    root = node;
                }
                else if (current is IAstExpectValue)
                {
                    current.AddChild(node);
                    current = node;
                }
                else if (node is BinaryOperator op)
                {
                    while (current.Parent is UnaryOperator or AstSequence)
                    {
                        current = current.Parent;
                    }
                    while ((current.Parent as BinaryOperator)?.ComparePrecedence(op) > 0)
                    {
                        current = current.Parent;
                    }
                    current.Parent?.Replace(current, op);
                    op.AddChild(current);
                    if (current == root) root = op;
                    current = op;
                }
                else if (current.Parent is AstSequence {TopLevel: false} sequence)
                {
                    sequence.AddChild(node);
                    current = node;
                }
                else
                {
                    var components = new List<AstNode> {current, node};
                    var inner = new AstSequence(components);
                    current.Parent?.Replace(current, inner);
                    current.Parent = inner;
                    if (current == root) root = inner;
                    current = inner.Last;
                }
            }

            for (var i = start; i < tokens.Length; i++)
            {
                var (content, type) = tokens[i];
                if (content == split)
                {
                    if (current == null)
                    {
                        root = current = new AstSequence(new List<AstNode> {null}, true);
                        continue;
                    }
                    while (current.Parent != null)
                    {
                        current = current.Parent;
                    }
                    if (current is not AstSequence {TopLevel: true})
                    {
                        root = current = new AstSequence(new List<AstNode> {current}, true);
                    }
                }
                else if (currentGroup is not null && content == currentGroup.Right && (currentGroup.Left != currentGroup.Right || current is not IAstExpectValue))
                {
                    
                }
                else if (type == TokenType.Symbol)
                {
                    if (groups.TryGetValue(content, out var groupSymbol))
                    {
                        if (groupSymbol.Left != groupSymbol.Right || current is null or IAstExpectValue || content != currentGroup?.Right)
                        {
                            var inside = Read(tokens, i + 1, out i, groupSymbol);
                            var group = new AstGroup(groupSymbol, inside);
                            Insert(group);
                        }
                        else
                        {
                            end = i;
                            break;
                        }
                    }
                    if (unary.TryGetValue(content, out var unarySymbol) &&
                        current is null or IAstExpectValue)
                    {
                        var unaryOperator = new UnaryOperator(unarySymbol);
                        current?.AddChild(unaryOperator);
                        current = unaryOperator;
                    }
                    if (binary.TryGetValue(content, out var binarySymbol))
                    {
                        //
                    }
                }
            }
        }
    }

    public record BinarySymbol(string Symbol, int Precedence, bool LeftAssociative);

    public record UnarySymbol(string Symbol);

    public record GroupSymbol(string Left, string Right)
    {
        public GroupSymbol(string symbol) : this(symbol, symbol) { }

        public bool Contains(string symbol) => Left.Equals(symbol) || Right.Equals(symbol);
    }
    
    public interface IAstExpectValue { }

    public abstract class AstNode
    {
        public AstNode Parent;

        public abstract void AddChild(AstNode node);

        public abstract void Replace(AstNode old, AstNode node);

        protected void SetChild(AstNode parent, ref AstNode node, AstNode value)
        {
            if (node != null) node.Parent = null;
            node = value;
            node.Parent = parent;
        }
    }

    public abstract class AstLeaf : AstNode
    {
        public override void AddChild(AstNode node) => throw new NotSupportedException("Node cannot have children.");

        public override void Replace(AstNode old, AstNode node) => throw new NotSupportedException("Node cannot have children.");
    }

    public class AstValue : AstLeaf
    {
        public readonly string Value;

        public AstValue(string value) => Value = value;
    }

    public class BinaryOperator : AstNode, IAstExpectValue
    {
        public readonly BinarySymbol Symbol;
        public AstNode Left;
        public AstNode Right;

        public BinaryOperator(BinarySymbol symbol) => Symbol = symbol;

        public override void AddChild(AstNode node)
        {
            if (Left == null) SetChild(this, ref Left, node);
            else if (Right == null) SetChild(this, ref Right, node);
            else throw new Exception("Node is at maximum capacity.");
        }

        public override void Replace(AstNode old, AstNode node)
        {
            if (Left == old) SetChild(this, ref Left, node);
            else if (Right == old) SetChild(this, ref Right, node);
        }

        public int ComparePrecedence(BinaryOperator other)
        {
            if (other.Symbol.LeftAssociative) return Symbol.Precedence - other.Symbol.Precedence + 1;
            return Symbol.Precedence - other.Symbol.Precedence;
        }
    }

    public class UnaryOperator : AstNode, IAstExpectValue
    {
        public readonly UnarySymbol Symbol;
        public AstNode Value;

        public UnaryOperator(UnarySymbol symbol) => Symbol = symbol;

        public override void AddChild(AstNode node)
        {
            if (Value == null) SetChild(this, ref Value, node);
            else throw new Exception("Node is at maximum capacity.");
        }

        public override void Replace(AstNode old, AstNode node)
        {
            if (Value == old) SetChild(this, ref Value, node);
        }
    }

    public class AstGroup : AstNode
    {
        public readonly GroupSymbol Symbol;
        public readonly AstNode Content;

        public AstGroup(GroupSymbol symbol, AstNode content)
        {
            Symbol = symbol;
            SetChild(this, ref Content, content);
        }

        public override void AddChild(AstNode node) => throw new NotSupportedException();

        public override void Replace(AstNode old, AstNode node) => throw new NotSupportedException();
    }

    public class AstSequence : AstNode, IAstExpectValue
    {
        public readonly List<AstNode> Components;
        public readonly bool TopLevel;

        public AstSequence(List<AstNode> components, bool topLevel = false)
        {
            Components = components;
            TopLevel = topLevel;
            foreach (var node in components)
            {
                node.Parent = this;
            }
        }

        public AstNode Last => Components[^1];

        public override void AddChild(AstNode node)
        {
            AstNode child = null;
            SetChild(this, ref child, node);
            Components.Add(child);
        }

        public override void Replace(AstNode old, AstNode node) => throw new NotSupportedException();

        public void ReplaceLast(AstNode node)
        {
            Components[^1].Parent = null;
            node.Parent = this;
            Components[^1] = node;
        }
    }
}