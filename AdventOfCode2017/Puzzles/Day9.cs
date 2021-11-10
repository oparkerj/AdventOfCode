using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Parsing;

namespace AdventOfCode2017.Puzzles
{
    public class Day9 : Puzzle
    {
        public Day9()
        {
            Part = 2;
            // InputName = "Day9Ex.txt";
        }

        private IData ParseInput()
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

            return parser.Parse(reader.Read(InputLine, true));
        }

        public override void PartOne()
        {
            var data = ParseInput() as Group;
            WriteLn(data?.Score());
        }

        public override void PartTwo()
        {
            // > 5043
            var data = ParseInput() as Group;
            WriteLn(data?.GarbageLength());
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
    }
}