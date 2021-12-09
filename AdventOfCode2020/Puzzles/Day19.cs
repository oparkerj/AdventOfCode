using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities.Automata;
using RegExtract;

namespace AdventOfCode2020.Puzzles;

public class Day19 : Puzzle
{
    public new string[][] Groups;
    public Dictionary<int, string> Rules = new();

    public Dictionary<int, StateMachineHelper<char>> Matchers = new();

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

    public StateMachineHelper<char> Matcher() => new();

    public StateMachineHelper<char> Rule(int rule)
    {
        if (Matchers.TryGetValue(rule, out var matcher)) return matcher;
        matcher = Matcher();
        Matchers[rule] = matcher;
        var r = Rules[rule];
        if (r.StartsWith('"'))
        {
            var c = r[1];
            matcher = matcher.Then(c);
        }
        else
        {
            var branches = r.Split('|').Select(s => s.Trim().Split(' ').Ints()).Select(ints =>
            {
                var h = Matcher();
                h = ints.Select(Rule).Aggregate(h, (sm, part) => sm.Recurse(part));
                return h.Finish();
            });
            matcher = matcher.Choice(branches);
        }
        return matcher.Finish();
    }

    public override void PartOne()
    {
        var helper = Rule(0);
        // Require that the entire string matches
        var machine = helper.Continue().Ending().Build();
        WriteLn(Groups[1].Count(s => machine.Match(s, out _)));
    }

    public override void PartTwo()
    {
        // Update rules and run part 1 again
        Rules[8] = "42 | 42 8";
        Rules[11] = "42 31 | 42 11 31";
        PartOne();
    }
}