using System.Collections.Generic;
using AdventToolkit;
using AdventToolkit.Utilities;

namespace AdventOfCode2019.Puzzles
{
    public class Day14 : Puzzle
    {
        public record Reaction(List<(int amount, string type)> Inputs, int Amount, string Output);

        public Day14()
        {
            Part = 2;
        }

        public QuantityTreeOld<string> GetReactions()
        {
            return Input.ToQuantityTree(@"(?<Children>(\d+) (\w+)(?:, )?)+ => (?<Amount>\d+) (?<Value>\w+)");
        }

        public override void PartOne()
        {
            var tree = GetReactions();
            var counts = tree.Produce("FUEL");
            WriteLn(counts["ORE"]);
        }

        public override void PartTwo()
        {
            var tree = GetReactions();
            var amount = tree.ProduceUsing("FUEL", "ORE", 1_000_000_000_000);
            WriteLn(amount);
        }
    }
}