using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Parsing;

namespace AdventOfCode2017.Puzzles;

public class Day9 : Puzzle
{
    public Day9()
    {
        Part = 2;
    }

    private (int Score, int Garbage) Compute()
    {
        var depth = 1;
        var score = 0;
        var garbage = 0;
        var g = false;
        for (var i = 0; i < InputLine.Length; i++)
        {
            var c = InputLine[i];
            if (c == '!') i++;
            else if (g && c != '>') garbage++;
            else if (c == '<') g = true;
            else if (c == '>') g = false;
            else if (c == '{') score += depth++;
            else if (c == '}') depth--;
        }
        return (score, garbage);
    }

    public override void PartOne()
    {
        // var data = ReadInput() as Group;
        // WriteLn(data?.Score());

        var (score, _) = Compute();
        WriteLn(score);
    }

    public override void PartTwo()
    {
        // var data = ReadInput() as Group;
        // WriteLn(data?.GarbageLength());

        var (_, garbage) = Compute();
        WriteLn(garbage);
    }

    #region Overcomplicated Solution

    private IData ReadInput()
    {
        var reader = new AstReader();
        reader.Escape = "!";
        reader.EscapeBehavior = AstReader.EscapeHandling.Skip;
        reader.SequenceSplit = ",";
        var group = new GroupSymbol("{", "}");
        var garbage = new GroupSymbol("<", ">", false);
        reader.AddGroup(group).AddGroup(garbage);

        var parser = new AstParser<IData>();
        parser.Add(new TupleParser<IData>(group, (IList<IData> args, out IData result) =>
        {
            result = new Group {Data = args.ToList()};
            return true;
        }));
        parser.Add(new TupleParser<IData>(garbage, (IList<IData> args, out IData result) =>
        {
            result = new Garbage {Content = args.Count > 0 ? args[0] as GarbageData : null};
            return true;
        }));
        parser.Add(new SequenceParser<IData>((AstParser<IData> _, IList<AstNode> nodes, out IData result) =>
        {
            result = new GarbageData(nodes.Str());
            return true;
        }));

        var nodes = reader.Read(InputLine, new TokenSettings {SingleLetters = true});
        return parser.Parse(nodes);
    }

    private interface IData { }

    private class Group : IData
    {
        public List<IData> Data { get; init; }

        public int Score(int current = 1)
        {
            var sum = Data.WhereType<Group>()
                .Select(g => g.Score(current + 1))
                .Sum();
            return current + sum;
        }

        public int GarbageLength()
        {
            return Data.Select(data =>
            {
                if (data is Group group) return group.GarbageLength();
                if (data is Garbage garbage) return garbage.Content?.Value.Length ?? 0;
                return 0;
            }).Sum();
        }
    }

    private class Garbage : IData
    {
        public GarbageData Content { get; init; }
    }

    private class GarbageData : IData
    {
        public readonly string Value;

        public GarbageData(string value) => Value = value;
    }

    #endregion
}