using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit;

namespace AdventOfCode2020.Puzzles
{
    public class Day7 : Puzzle
    {
        public Day7()
        {
            Part = 2;
        }

        private Dictionary<string, (string, int)[]> bags;

        public void ReadBags()
        {
            bags = new Dictionary<string, (string, int)[]>();
            foreach (var type in Input)
            {
                var i = type.IndexOf(" bags", StringComparison.Ordinal);
                var name = type[..i];
                var content = type[(i + 14)..^1];
                if (content == "no other bags")
                {
                    bags[name] = null;
                    continue;
                }
                var contains = content.Split(", ");
                var holds = contains.Select(s =>
                {
                    var j = s.IndexOf(' ');
                    var amount = int.Parse(s[..j]);
                    var end = amount > 1 ? 5 : 4;
                    return (s[(j + 1)..^end], amount);
                }).ToArray();
                bags[name] = holds;
            }
        }

        public Dictionary<string, int> GetContents(string type)
        {
            var content = new Dictionary<string, int>();
            var types = bags[type];
            if (types == null) return content;
            foreach (var (name, amount) in types)
            {
                content.TryGetValue(name, out var c);
                content[name] = c + amount;
                var inner = GetContents(name);
                foreach (var (key, value) in inner)
                {
                    content.TryGetValue(key, out c);
                    content[key] = c + value * amount;
                }
            }
            return content;
        }
        
        public override void PartOne()
        {
            ReadBags();
            var count = bags.Keys.Select(GetContents).Count(content => content.ContainsKey("shiny gold"));
            WriteLn(count);
        }

        public override void PartTwo()
        {
            ReadBags();
            var sum = GetContents("shiny gold").Values.Sum();
            WriteLn(sum);
        }
    }
}