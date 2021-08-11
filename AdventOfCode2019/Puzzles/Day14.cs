using System.Collections.Generic;
using AdventToolkit;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2019.Puzzles
{
    public class Day14 : Puzzle
    {
        public record Reaction(List<(int amount, string type)> Inputs, int Amount, string Output);

        public Day14()
        {
            Part = 2;
        }

        public void BuildTree(Reaction reaction, QuantityTreeHelper<string> helper)
        {
            helper.Add(reaction.Output, reaction.Amount);
            foreach (var (amount, type) in reaction.Inputs)
            {
                helper.AddChild(type, amount);
            }
        }
        
        public override void PartOne()
        {
            var tree = Input.Extract<Reaction>(@"((\d+) (\w+)(?:, )?)+ => (\d+) (\w+)")
                .ToQuantityTree<Reaction, string>(BuildTree);
            var counts = tree.Produce("FUEL", 1);
            WriteLn(counts["ORE"]);
        }

        public override void PartTwo()
        {
            var tree = Input.Extract<Reaction>(@"((\d+) (\w+)(?:, )?)+ => (\d+) (\w+)")
                .ToQuantityTree<Reaction, string>(BuildTree);
            var amount = tree.ProduceUsing("FUEL", "ORE", 1_000_000_000_000);
            WriteLn(amount);
        }
    }
}