using AdventToolkit;
using AdventToolkit.Collections.Tree;
using AdventToolkit.Extensions;

namespace AdventOfCode2019.Puzzles;

public class Day14 : Puzzle
{
    public Day14()
    {
        Part = 2;
    }

    public QuantityTree<string, long> GetReactions()
    {
        return Input.ToQuantityTree<long>(@"(?<Children>(\d+) (\w+)(?:, )?)+ => (?<Amount>\d+) (?<Value>\w+)");
    }

    public override void PartOne()
    {
        var tree = GetReactions();
        var counts = tree.Produce("FUEL", 1);
        WriteLn(counts["ORE"]);
    }

    public override void PartTwo()
    {
        var tree = GetReactions();
        var amount = tree.ProduceUsing("FUEL", "ORE", 1_000_000_000_000);
        WriteLn(amount);
    }
}