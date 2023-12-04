using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
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
            .Select(pair => pair.Value.Values.Product())
            .Sum();
    }
}