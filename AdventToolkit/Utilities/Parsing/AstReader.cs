using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Parsing;

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
    // true = Closing group symbol always tries to close a group, otherwise only the matching symbol can close a group
    // e.g. "(>)" strict = false, ok. strict = true, mismatched group
    public bool StrictGroups;
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

    public AstNode Read(string s, TokenSettings settings = default)
    {
        if (!TryRead(s.Tokenize(settings).ToArray(), out var node, out var error)) throw new Exception(error.ToString());
        return node;
    }

    public bool TryRead(Token[] tokens, out AstNode node, out AstError error)
    {
        var unary = UnarySymbols;
        var binary = BinarySymbols;
        var groups = GroupSymbols;
        var split = SequenceSplit;
        var escape = Escape;
        var err = AstErrorReason.None;
            
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

        AstErrorReason CheckState(int index)
        {
            if (currents.Peek() is IAstExpectValue) return AstErrorReason.ExpectedValue;
            if (currentGroups.Peek() is not null && index == tokens.Length) return AstErrorReason.UnclosedGroup;
            return AstErrorReason.None;
        }

        AstErrorReason PopLevel(int index)
        {
            var e = CheckState(index);
            if (e != AstErrorReason.None) return e;
            var group = new AstGroup(currentGroups.Peek(), roots.Peek());
            roots.Pop();
            currents.Pop();
            currentGroups.Pop();
            Insert(group);
            return AstErrorReason.None;
        }

        var index = 0;
        for (index = 0; index < tokens.Length; index++)
        {
            var token = tokens[index];
            var (content, type) = token;
            var currentGroup = currentGroups.Peek();
            if (content == escape)
            {
                if (EscapeBehavior == EscapeHandling.None)
                {
                    Insert(new AstValue(token));
                    Insert(new AstValue(tokens[++index]));
                }
                else if (EscapeBehavior == EscapeHandling.Value)
                {
                    Insert(new AstValue(tokens[++index]));
                }
                else if (EscapeBehavior == EscapeHandling.Skip)
                {
                    index++;
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
            else if (currentGroup is not null && (StrictGroups ? groups.Any(pair => content == pair.Value.Right) : content == currentGroup.Right) &&
                     (currentGroup.Left != currentGroup.Right || currents.Peek() is not null and not IAstExpectValue))
            {
                if (content != currentGroup.Right)
                {
                    err = AstErrorReason.MismatchedGroup;
                    goto HandleError;
                }
                // Reached end of group or assumed end of group for ambiguous tokens.
                err = PopLevel(index);
                if (err != AstErrorReason.None) goto HandleError;
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
                        err = PopLevel(index);
                        if (err != AstErrorReason.None) goto HandleError;
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
                    if (currents.Peek() is null or IAstExpectValue)
                    {
                        err = AstErrorReason.InvalidBinaryOperator;
                        goto HandleError;
                    }
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
        err = CheckState(tokens.Length);

        HandleError:
        if (err != AstErrorReason.None)
        {
            node = default;
            error = new AstError(err, tokens, index, currentGroups);
            return false;
        }

        node = roots.Peek();
        error = default;
        return true;
    }

    public enum EscapeHandling
    {
        None,
        Value,
        Skip,
    }
}

public enum AstErrorReason
{
    None,
    ExpectedValue,
    UnclosedGroup,
    InvalidBinaryOperator,
    MismatchedGroup
}

public class AstError
{
    public readonly AstErrorReason Reason;
    public readonly Token[] Tokens;
    public readonly int Index;
    public readonly Stack<GroupSymbol> Groups;

    public AstError(AstErrorReason reason, Token[] tokens, int index, Stack<GroupSymbol> groups)
    {
        Reason = reason;
        Tokens = tokens;
        Index = index;
        Groups = groups;
    }

    public IEnumerable<GroupSymbol> UnclosedGroups => Groups.Take(Groups.Count - 1);

    public Token CurrentToken => Tokens[Index];
    public string Content => CurrentToken.Content;

    public override string ToString()
    {
        return Reason switch
        {
            AstErrorReason.ExpectedValue => $"Value expected at token {Index}",
            AstErrorReason.UnclosedGroup => $"Unclosed group \"{Groups.Peek()?.Left}\"",
            AstErrorReason.InvalidBinaryOperator => $"Invalid binary operator {Tokens[Index]} at token {Index}",
            AstErrorReason.MismatchedGroup => $"Found \"{Tokens[Index]}\" when expecting closing group for \"{Groups.Peek()?.Left}\"",
            _ => ""
        };
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