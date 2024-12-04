using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit2.Calc;
using AdventToolkit2.Parsing;
using AdventToolkit2.Parsing.Builtin;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day2 : Puzzle<int>
{
    public Pair<int, Dictionary<string, int>> GameInfo(string game)
    {
        var id = game.Extract<int>(Patterns.Int);
        var shown = new Dictionary<string, int>();
        foreach (var hand in game.After(':').Split(';'))
        {
            var cubes = hand.Split(',').Extract<Pair<int, string>>(@"(\d+) (\w+)").ToKv().Swap();
            shown.Merge(cubes, Math.Max);
        }
        return (id, shown);
    }
    
    public override int PartOne()
    {
        var available = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        return Input.Select(GameInfo)
            .Where(pair => pair.Value.Le(available))
            .Keys()
            .Sum();
    }

    public override int PartTwo()
    {
        return Input.Select(GameInfo)
            .Select(pair => EnumerableExtensions.Product(pair.Value.Values))
            .Sum();
    }
}

public class Day2New : AdventToolkit2.Puzzle<int>
{
    public override string InputName() => $"{nameof(Day2)}.txt";

    public IEnumerable<(int, Dictionary<string, int>)> Games()
    {
        var toDict = new DictParseSwap<string, int>(Compare.Max);
        return Input.Parse<(int, Dictionary<string, int>)>($" {0}:{';'}{',':t}{' '}{typeof((int, string)):@-}{toDict:@-2;*;-}");
    }

    public override int PartOne()
    {
        var available = new Dictionary<string, int>
        {
            ["red"] = 12,
            ["green"] = 13,
            ["blue"] = 14
        };

        return Games()
            .Where(pair => pair.Item2.Le(available))
            .Select(tuple => tuple.Item1)
            .Sum();
    }

    public override int PartTwo()
    {
        return Games()
            .Select(tuple => EnumerableExtensions.Product(tuple.Item2.Values))
            .Sum();
    }
}