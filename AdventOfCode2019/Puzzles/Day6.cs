using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles
{
    public class Day6 : Puzzle
    {
        public Day6()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var tree = Input.ToTree(s => s[..3], s => s[4..]);
            var sum = tree.Select(node => node.Height).Sum();
            WriteLn(sum);
        }

        public override void PartTwo()
        {
            var tree = Input.ToTree(s => s[..3], s => s[4..]);
            var you = tree["YOU"].Height;
            var san = tree["SAN"].Height;
            var common = tree.CommonAncestor("YOU", "SAN").Height;
            WriteLn(you + san - 2 - common * 2);
        }
    }
}