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

        public string ToRegex(int rule)
        {
            var spec = Rules[rule];
            if (spec.Search('|', out var i))
            {
                return $"({ToRegex(spec[..i])}|{ToRegex(spec[(i + 1)..])})";
            }
            return $"{ToRegex(spec)}";
        }

        public string ToRegex(string part)
        {
            part = part.Trim();
            if (part.StartsWith('"')) return part[1].ToString();
            var parts = part.Split(' ');
            return string.Concat(parts.Select(int.Parse).Select(ToRegex));
        }
        
        public override void PartOne()
        {
            var regex = ToRegex(0);
            var r = new Regex($"^{regex}$", RegexOptions.Compiled);
            var result = Groups[1].Count(r.IsMatch);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            Rules[8] = "\":\"";
            Rules[11] = "\";\"";
            var regex = ToRegex(0);
            
            regex = regex.Replace(":", $"({ToRegex(42)})+");
            
            // Brute force every possible number of repetitions within the input
            var longest = Groups[1].Select(s => s.Length).Max();
            var counts = Enumerable.Range(0, longest).Select(c =>
            {
                return $"{ToRegex(42)}{{{c + 1}}}{ToRegex(31)}{{{c + 1}}}";
            });
            regex = regex.Replace(";", $"({string.Join('|', counts)})");
            
            var r = new Regex($"^{regex}$", RegexOptions.Compiled);
            var result = Groups[1].Count(r.IsMatch);
            WriteLn(result);
        }
    }
}