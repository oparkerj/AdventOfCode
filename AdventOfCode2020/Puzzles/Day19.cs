using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2020.Puzzles
{
    public class Day19 : Puzzle
    {
        public string[][] Groups;
        public Dictionary<int, string> Rules = new();

        public Day19()
        {
            Groups = base.Groups.ToArray();
            ReadRules();
            Part = 2;
        }

        public void ReadRules()
        {
            var rules = Groups[0].Extract<(int, string)>(@"(\d+): (.+)");
            foreach (var (rule, spec) in rules)
            {
                Rules[rule] = spec;
            }
        }

        public string ToRegex(int rule, bool part2 = false)
        {
            if (part2)
            {
                if (rule == 8) return $"({ToRegex(42)})+";
                if (rule == 11)
                {
                    // Brute force every possible number of repetitions within the input
                    var longest = Groups[1].Select(s => s.Length).Max();
                    var counts = Enumerable.Range(0, longest).Select(c =>
                    {
                        return $"{ToRegex(42)}{{{c + 1}}}{ToRegex(31)}{{{c + 1}}}";
                    });
                    return $"({string.Join('|', counts)})";
                }
            }
            var spec = Rules[rule];
            if (spec.Search('|', out var i))
            {
                return $"({ToRegex(spec[..i], part2)}|{ToRegex(spec[(i + 1)..], part2)})";
            }
            return $"{ToRegex(spec, part2)}";
        }

        public string ToRegex(string part, bool part2)
        {
            part = part.Trim();
            if (part.StartsWith('"')) return part[1].ToString();
            var parts = part.Split(' ');
            return string.Concat(parts.Select(int.Parse).Select(i => ToRegex(i, part2)));
        }
        
        public override void PartOne()
        {
            var regex = ToRegex(0, Part == 2);
            var r = new Regex($"^{regex}$", RegexOptions.Compiled);
            var result = Groups[1].Count(r.IsMatch);
            WriteLn(result);
        }
    }
}