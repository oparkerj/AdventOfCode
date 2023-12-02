using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day2 : Puzzle<int>
{
    public KeyValuePair<int, Dictionary<string, int>> GameInfo(string game)
    {
        var id = game.Extract<int>(Patterns.Int);
        var shown = new Dictionary<string, int>();
        foreach (var hand in game.After(':').Split(';'))
        {
            foreach (var (value, key) in hand.Split(',').Extract<KeyValuePair<int, string>>(@"(\d+) (\w+)"))
            {
                if (!shown.TryAdd(key, value))
                {
                    shown[key] = Math.Max(shown[key], value);
                }
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
            .Where(pair => pair.Value.All(count => available[count.Key] >= count.Value))
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