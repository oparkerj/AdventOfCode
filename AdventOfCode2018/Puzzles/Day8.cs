using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections;
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

        public Tree<int[]> GetTree()
        {
            var input = InputLine.Split(' ').Ints().ToArray();
            var tree = new Tree<int[]>();

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
                    node.LinkTo(tree[id]);
                }
                node.Value = data[start..(start + meta)];
                return (start + meta - beginning, node.Id);
            }

            Read(input, 0);
            return tree;
        }

        public override void PartOne()
        {
            var tree = GetTree();
            var result = tree.SelectMany(node => node.Value).Sum();
            WriteLn(result);
        }

        public override void PartTwo()
        {
            static int Value(TreeVertex<int[], Edge<int[]>> vertex)
            {
                if (vertex.Count == 0) return vertex.Value.Sum();
                var links = vertex.Neighbors.ToList();
                var range = Interval.Range(0, links.Count);
                return vertex.Value.Select(i => i - 1)
                    .Where(range.Contains)
                    .GetFrom(links)
                    .Select(Value)
                    .Sum();
            }
            
            WriteLn(Value(GetTree().Root));
        }
    }
}