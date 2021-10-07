using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2018.Puzzles
{
    public class Day8 : Puzzle
    {
        public Day8()
        {
            Part = 2;
        }

        public DataTreeOld<int[]> GetTree()
        {
            var input = InputLine.Split(' ').Ints().ToArray();
            var tree = new DataTreeOld<int[]>();

            (int, int) Read(int[] data, int start)
            {
                var beginning = start;
                var node = tree.NewNode();
                var children = data[start];
                var meta = data[start + 1];
                start += 2;
                for (var i = 0; i < children; i++)
                {
                    var (length, id) = Read(data, start);
                    start += length;
                    node.AddChild(tree[id]);
                }
                node.Data = data[start..(start + meta)];
                return (start + meta - beginning, node.Value);
            }

            Read(input, 0);
            return tree;
        }

        public override void PartOne()
        {
            var tree = GetTree();
            var result = tree.SelectMany(node => node.Data).Sum();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            static int Value(DataNode<int[]> node)
            {
                if (node.Count == 0) return node.Data.Sum();
                var links = node.ChildLinks;
                var range = Interval.Range(0, links.Count);
                return node.Data.Select(i => i - 1)
                    .Where(range.Contains)
                    .GetFrom(links)
                    .Select(Value)
                    .Sum();
            }
            
            WriteLn(Value(GetTree().Root));
        }
    }
}