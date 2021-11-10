using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Parsing
{
    // Parse a string into an AST tree.
    // Parses basic structures including binary operators, unary operators,
    // and delimited groups.
    public class AstReader
    {
        public readonly Dictionary<string, BinarySymbol> BinarySymbols;
        public readonly Dictionary<string, UnarySymbol> UnarySymbols;
        public readonly Dictionary<string, GroupSymbol> GroupSymbols;
        public string SequenceSplit;
        public string Escape;
        public EscapeHandling EscapeBehavior = EscapeHandling.Value;

        public AstReader()
        {
            BinarySymbols = new Dictionary<string, BinarySymbol>();
            UnarySymbols = new Dictionary<string, UnarySymbol>();
            GroupSymbols = new Dictionary<string, GroupSymbol>();
        }

        public AstReader AddGroup(GroupSymbol symbol)
        {
            GroupSymbols[symbol.Left] = symbol;
            return this;
        }

        public AstNode Read(string s, bool keepWhitespace = false) => Read(s.Tokenize(keepWhitespace).ToArray());
        
        private AstNode Read(Token[] tokens)
        {
            var unary = UnarySymbols;
            var binary = BinarySymbols;
            var groups = GroupSymbols;
            var split = SequenceSplit;
            var escape = Escape;
            
            var roots = new Stack<AstNode>();
            var currents = new Stack<AstNode>();
            var currentGroups = new Stack<GroupSymbol>();

            roots.Push(null);
            currents.Push(null);
            currentGroups.Push(null);

            void Insert(AstNode node)
            {
                if (currents.Peek() == null)
                {
                    currents.Replace(node);
                    roots.Replace(node);
                }
                else if (currents.Peek() is IAstExpectValue)
                {
                    currents.Peek().AddChild(node);
                    currents.Replace(node);
                }
                else if (node is BinaryOperator op)
                {
                    var cur = currents.Peek();
                    while (cur.Parent is UnaryOperator or AstSequence)
                    {
                        cur = cur.Parent;
                    }
                    while ((cur.Parent as BinaryOperator)?.ComparePrecedence(op) > 0)
                    {
                        cur = cur.Parent;
                    }
                    cur.Parent?.Replace(cur, op);
                    op.AddChild(cur);
                    if (cur == roots.Peek()) roots.Replace(op);
                    currents.Replace(op);
                }
                else if (currents.Peek().Parent is AstSequence {TopLevel: false} sequence)
                {
                    sequence.AddChild(node);
                    currents.Replace(node);
                }
                else
                {
                    var cur = currents.Peek();
                    var parent = cur.Parent;
                    var components = new List<AstNode> {cur, node};
                    var inner = new AstSequence(components, "");
                    parent?.Replace(cur, inner);
                    cur.Parent = inner;
                    if (cur == roots.Peek()) roots.Replace(inner);
                    currents.Replace(inner.Last);
                }
            }

            void CheckState(int index)
            {
                if (currents.Peek() is IAstExpectValue) throw new Exception("Invalid input.");
                if (currentGroups.Peek() is not null && index == tokens.Length) throw new Exception("Unclosed group.");
            }

            void PopLevel(int index)
            {
                CheckState(index);
                var group = new AstGroup(currentGroups.Peek(), roots.Peek());
                roots.Pop();
                currents.Pop();
                currentGroups.Pop();
                Insert(group);
            }

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                var (content, type) = token;
                var currentGroup = currentGroups.Peek();
                if (content == escape)
                {
                    if (EscapeBehavior == EscapeHandling.Value)
                    {
                        Insert(new AstValue(tokens[++i]));
                    }
                    else if (EscapeBehavior == EscapeHandling.Skip)
                    {
                        i++;
                    }
                }
                else if (content == split && (currentGroup is null || currentGroup.Nest))
                {
                    // Set up for top-level sequence
                    if (currents.Peek() == null)
                    {
                        var seq = new AstSequence(new List<AstNode> {null}, split, true);
                        currents.Replace(seq);
                        roots.Replace(seq);
                        continue;
                    }
                    var cur = currents.Peek();
                    while (cur.Parent != null)
                    {
                        cur = cur.Parent;
                    }
                    if (cur is not AstSequence {TopLevel: true})
                    {
                        var seq = new AstSequence(new List<AstNode> {cur}, split, true);
                        roots.Replace(seq);
                        currents.Replace(seq);
                    }
                    else currents.Replace(cur);
                }
                else if (currentGroup is not null && content == currentGroup.Right &&
                         (currentGroup.Left != currentGroup.Right || currents.Peek() is not null and not IAstExpectValue))
                {
                    // Reached end of group or assumed end of group for ambiguous tokens.
                    PopLevel(i);
                }
                else if (type == TokenType.Symbol)
                {
                    if ((currentGroup is null || currentGroup.Nest) && groups.TryGetValue(content, out var groupSymbol))
                    {
                        if (groupSymbol.Left != groupSymbol.Right || currents.Peek() is null or IAstExpectValue || content != currentGroup?.Right)
                        {
                            roots.Push(null);
                            currents.Push(null);
                            currentGroups.Push(groupSymbol);
                        }
                        else
                        {
                            PopLevel(i);
                        }
                    }
                    else if (unary.TryGetValue(content, out var unarySymbol) &&
                             currents.Peek() is null or IAstExpectValue)
                    {
                        var unaryOperator = new UnaryOperator(unarySymbol);
                        currents.Peek()?.AddChild(unaryOperator);
                        currents.Replace(unaryOperator);
                    }
                    else if (binary.TryGetValue(content, out var binarySymbol))
                    {
                        if (currents.Peek() is null or IAstExpectValue) throw new Exception("Invalid position for binary operator.");
                        Insert(new BinaryOperator(binarySymbol));
                    }
                    else
                    {
                        Insert(new AstValue(token));
                    }
                }
                else
                {
                    Insert(new AstValue(token));
                }
            }
            CheckState(tokens.Length);
            return roots.Peek();
        }

        public enum EscapeHandling
        {
            Value,
            Skip,
        }
    }

    public record BinarySymbol(string Symbol, int Precedence, bool LeftAssociative = true);

    public record UnarySymbol(string Symbol);

    public record GroupSymbol(string Left, string Right, bool Nest = true)
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
            if (node != null) node.Parent = parent;
        }
    }

    public abstract class AstLeaf : AstNode
    {
        public override void AddChild(AstNode node) => throw new NotSupportedException("Node cannot have children.");

        public override void Replace(AstNode old, AstNode node) => throw new NotSupportedException("Node cannot have children.");
    }

    public class AstValue : AstLeaf
    {
        public readonly Token Value;

        public AstValue(Token value) => Value = value;

        public override string ToString() => Value.Content;
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

        public override string ToString()
        {
            return $"{Left} {Symbol.Symbol} {Right}";
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

        public override string ToString()
        {
            return $"{Symbol.Symbol}{Value}";
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

        public override string ToString()
        {
            return $"{Symbol.Left}{Content}{Symbol.Right}";
        }
    }

    public class AstSequence : AstNode, IAstExpectValue
    {
        public readonly List<AstNode> Components;
        public readonly string Split;
        public readonly bool TopLevel;

        public AstSequence(List<AstNode> components, string split, bool topLevel = false)
        {
            Components = components;
            Split = split;
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

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Split)) return string.Concat(Components);
            return string.Join(Split, Components);
        }
    }
}