using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities.Parsing;

namespace AdventOfCode2021.Puzzles;

public class Day10 : Puzzle
{
    public Day10()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var pairs = new Dictionary<char, char>
        {
            ['('] = ')',
            ['['] = ']',
            ['{'] = '}',
            ['<'] = '>',
        };
        var points = new Dictionary<char, int>
        {
            [')'] = 3,
            [']'] = 57,
            ['}'] = 1197,
            ['>'] = 25137,
        };

        var total = 0;
        foreach (var line in Input)
        {
            var s = new Stack<char>();
            foreach (var c in line)
            {
                if (pairs.ContainsKey(c)) s.Push(c);
                else
                {
                    var expect = s.Pop();
                    if (pairs[expect] != c) total += points[c];
                }
            }
        }
        WriteLn(total);
    }

    public override void PartTwo()
    {
        var pairs = new Dictionary<char, char>
        {
            ['('] = ')',
            ['['] = ']',
            ['{'] = '}',
            ['<'] = '>',
        };
        var points = new Dictionary<char, int>
        {
            ['('] = 1,
            ['['] = 2,
            ['{'] = 3,
            ['<'] = 4,
        };

        var scores = new List<long>();
        foreach (var line in Input)
        {
            var s = new Stack<char>();
            foreach (var c in line)
            {
                if (pairs.ContainsKey(c)) s.Push(c);
                else
                {
                    var expect = s.Peek();
                    if (pairs[expect] != c) goto Corrupted;
                    s.Pop();
                }
            }
            goto Incomplete;
            
            Corrupted:
            continue;
            
            Incomplete:
            var score = 0L;
            while (s.Count > 0)
            {
                score = score * 5 + points[s.Pop()];
            }
            scores.Add(score);
        }
        WriteLn(scores.Sorted().ElementAt(scores.Count / 2));
    }
}

public class Day10V2 : Puzzle
{
    public Day10V2()
    {
        Part = 2;
        InputName = "Day10.txt";
    }

    public AstReader GetReader()
    {
        var reader = new AstReader();
        reader.AddGroup(new GroupSymbol("(", ")"));
        reader.AddGroup(new GroupSymbol("[", "]"));
        reader.AddGroup(new GroupSymbol("{", "}"));
        reader.AddGroup(new GroupSymbol("<", ">"));
        reader.StrictGroups = true;
        return reader;
    }

    public override void PartOne()
    {
        var reader = GetReader();
        var points = new Dictionary<char, int>
        {
            [')'] = 3,
            [']'] = 57,
            ['}'] = 1197,
            ['>'] = 25137,
        };

        var total = 0;
        foreach (var line in Input)
        {
            reader.TryRead(line.Tokenize().ToArray(), out _, out var error);
            if (error.Reason == AstErrorReason.MismatchedGroup)
            {
                total += points[error.Content[0]];
            }
        }
        WriteLn(total);
    }

    public override void PartTwo()
    {
        var reader = GetReader();
        var points = new Dictionary<char, int>
        {
            ['('] = 1,
            ['['] = 2,
            ['{'] = 3,
            ['<'] = 4,
        };

        var scores = new List<long>();
        foreach (var line in Input)
        {
            reader.TryRead(line.Tokenize().ToArray(), out _, out var error);
            if (error.Reason == AstErrorReason.UnclosedGroup)
            {
                var score = error.UnclosedGroups.Aggregate(0L, (score, symbol) => score * 5 + points[symbol.Left[0]]);
                scores.Add(score);
            }
        }
        WriteLn(scores.Sorted().ElementAt(scores.Count / 2));
    }
}