using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventToolkit;
using AdventToolkit.Data;
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

        public int SearchThrough(string s, int start, char c)
        {
            var level = 0;
            for (var i = start; i < s.Length; i++)
            {
                if (s[i] == c && level == 0) return i;
                if (s[i] == '(') level++;
                else if (s[i] == ')') level--;
            }
            return -1;
        }

        public string ToRegex(int rule)
        {
            var spec = Rules[rule];
            if (spec.Search('|', out var i))
            {
                return $"({ToRegex(spec[..i])}|{ToRegex(spec[(i + 1)..])})";
            }
            return ToRegex(spec);
        }

        public string ToRegex(string part)
        {
            part = part.Trim();
            if (part.StartsWith('"')) return part[1].ToString();
            var parts = part.Split(' ');
            return string.Concat(parts.Select(int.Parse).Select(ToRegex));
        }

        public int[] ToStates(string s, int[] from, StateMachine<char> machine)
        {
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (c == '(')
                {
                    var end = s.GetEndParen(i);
                    var mid = SearchThrough(s, i + 1, '|');
                    from = ToStates(s[(i + 1)..mid], from, machine)
                        .Concat(ToStates(s[(mid + 1)..end], from, machine))
                        .ToArray();
                    i = end;
                }
                else
                {
                    var next = machine.NewState();
                    foreach (var id in from)
                    {
                        machine[(id, c)].Add(next);
                    }
                    from = new[] {next};
                }
            }
            return from;
        }

        public override void PartOne()
        {
            var regex = ToRegex(0);

            var machine = new StateMachine<char>();
            machine.NewState(); // initial state
            var ends = ToStates(regex, new[] {0}, machine);
            machine.AcceptingStates.UnionWith(ends);
            if (machine.IsNfa()) machine = machine.NfaToDfa("ab");
            WriteLn(Groups[1].Count(machine.Test));

            var r = new Regex($"^{regex}$", RegexOptions.Compiled);
            var result = Groups[1].Count(r.IsMatch);
            WriteLn(result);
        }

        public override void PartTwo()
        {
            var rule42 = new StateMachine<char>();
            rule42.NewState(); // initial state
            var ends = ToStates(ToRegex(42), new[] {0}, rule42);
            rule42.AcceptingStates.UnionWith(ends);
            if (rule42.IsNfa()) rule42 = rule42.NfaToDfa("ab");

            var rule31 = new StateMachine<char>();
            rule31.NewState();
            ends = ToStates(ToRegex(31), new[] {0}, rule31);
            rule31.AcceptingStates.UnionWith(ends);
            if (rule31.IsNfa()) rule31 = rule31.NfaToDfa("ab");

            // Count number of times rule 42 matches followed by number
            // of times rule 31 matches. The string matches if rule 42
            // occurs more than rule 31 and both match at least once
            var count = Groups[1]
                .Where(s =>
                {
                    var (count42, end42) = rule42.Count(s);
                    var (count31, end31) = rule31.Count(s[end42..]);
                    if (end42 + end31 != s.Length) return false;
                    return count31 >= 1 && count42 > count31;
                })
                .Count();
            WriteLn(count);
        }

        public void OldPartTwo()
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