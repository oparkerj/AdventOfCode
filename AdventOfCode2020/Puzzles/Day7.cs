using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Tree;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles;

public class Day7 : Puzzle
{
    public Day7()
    {
        // ReadBags();
        Part = 2;
    }

    #region Old Solution

    // public Dictionary<string, (string, int)[]> Bags;
    //
    // public void ReadBags()
    // {
    //     Bags = new Dictionary<string, (string, int)[]>();
    //     foreach (var type in Input)
    //     {
    //         var i = type.IndexOf(" bags", StringComparison.Ordinal);
    //         var name = type[..i];
    //         var content = type[(i + 14)..^1];
    //         if (content == "no other bags")
    //         {
    //             Bags[name] = null;
    //             continue;
    //         }
    //         var contains = content.Split(", ");
    //         var holds = contains.Select(s =>
    //         {
    //             var j = s.IndexOf(' ');
    //             var amount = int.Parse(s[..j]);
    //             var end = amount > 1 ? 5 : 4;
    //             return (s[(j + 1)..^end], amount);
    //         }).ToArray();
    //         Bags[name] = holds;
    //     }
    // }

    // public Dictionary<string, int> GetContents(string type)
    // {
    //     var content = new Dictionary<string, int>();
    //     var types = Bags[type];
    //     if (types == null) return content;
    //     foreach (var (name, amount) in types)
    //     {
    //         content.TryGetValue(name, out var c);
    //         content[name] = c + amount;
    //         var inner = GetContents(name);
    //         foreach (var (key, value) in inner)
    //         {
    //             content.TryGetValue(key, out c);
    //             content[key] = c + value * amount;
    //         }
    //     }
    //     return content;
    // }

    #endregion

    public record Bag(string Name, List<(int Amount, string Bag)> Bags);

    public QuantityTree<string, long> GetBags()
    {
        return Input.Where(s => !s.Contains("no other bags"))
            .ToQuantityTree<long>(@"(?<Value>.+?) bags contain (?<WeightChild>(\d+) (.+?) bags?(?:, )?)+.");
    }
        
    public override void PartOne()
    {
        var count = GetBags().Count(node => node.HasChild("shiny gold"));
        WriteLn(count);
    }

    public override void PartTwo()
    {
        var count = GetBags().Produce("shiny gold", 1).Values.Sum() - 1;
        WriteLn(count);
    }
}