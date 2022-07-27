using AdventToolkit;
using AdventToolkit.Extensions;
using Newtonsoft.Json.Linq;

namespace AdventOfCode2015.Puzzles;

public class Day12 : Puzzle
{
    public int Sum(JToken token)
    {
        if (Part == 2 && token is JObject obj && obj.HasValue("red")) return 0;
        if (token.Type == JTokenType.Integer) return token.Value<int>();
        return token.Children().Select(Sum).Sum();
    }
    
    public override void PartOne()
    {
        var json = JToken.Parse(InputLine);
        WriteLn(Sum(json));
    }
}