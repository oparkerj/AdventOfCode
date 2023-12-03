using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day2 : Puzzle<int>
{
    // TODO make a better way to extract this data
    public KeyValuePair<int, Dictionary<string, int>> GameInfo(string game)
    {
        var id = game.Extract<int>(Patterns.Int);
        var shown = new Dictionary<string, int>();
        foreach (var hand in game.After(':').Split(';'))
        {
            foreach (var (value, key) in hand.Split(',').Extract<KeyValuePair<int, string>>(@"(\d+) (\w+)"))
            {
                shown[key] = Math.Max(shown.GetOrDefault(key, value), value);
            }
        }
        return new KeyValuePair<int, Dictionary<string, int>>(id, shown);
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