using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day19 : Puzzle<int, long>
{
    public new record Part(int X, int M, int A, int S);

    public record Condition(char Which, bool? LessThan, int Value, string Target)
    {
        public Condition Invert()
        {
            if (LessThan == true)
            {
                return this with {LessThan = false, Value = Value - 1};
            }
            if (LessThan == false)
            {
                return this with {LessThan = true, Value = Value + 1};
            }
            throw new Exception("If you see this then you big dummy");
        }
    }

    public record Workflow(Condition[] Conditions);

    public Dictionary<string, Workflow> GetWorkflows()
    {
        return Enumerable.ToDictionary(AllGroups[0].Select(s =>
        {
            var name = s.Before('{');
            var rules = s.Between('{', '}').Split(',').AsSpan();
            var @default = rules[^1];
            var conditions = new List<Condition>();
            foreach (var cond in rules[..^1])
            {
                bool? lt = cond[1] == '<';
                var value = cond[2..].Before(':').AsInt();
                var target = cond.After(':');
                conditions.Add(new Condition(cond[0], lt, value, target));
            }
            conditions.Add(new Condition(' ', null, 0, @default));
            return new KeyValuePair<string, Workflow>(name, new Workflow(conditions.ToArray()));
        }));
    }
    
    public bool IsAccepted(Part part, Dictionary<string, Workflow> workflows)
    {
        var workflow = "in";
        while (true)
        {
            workflow = GetNext(part, workflows[workflow]);
            if (workflow is "A") return true;
            if (workflow is "R") return false;
        }
    }
    
    public string GetNext(Part part, Workflow workflow)
    {
        var conditions = workflow.Conditions;
        foreach (var (which, lt, value, target) in conditions)
        {
            var left = which switch
            {
                'x' => part.X,
                'm' => part.M,
                'a' => part.A,
                's' => part.S,
            };
            if ((lt == true && left < value) || (lt == false && left > value) || lt == null)
            {
                return target;
            }
        }
        return string.Empty;
    }

    public override int PartOne()
    {
        var workflows = GetWorkflows();
        var parts = AllGroups[1].Extract<Part>(Patterns.Int4).ToList();

        return parts.Where(part => IsAccepted(part, workflows))
            .Select(part => part.X + part.M + part.A + part.S)
            .Sum();
    }

    public override long PartTwo()
    {
        var possible = Interval.RangeInclusive(1, 4000);
        var workflows = GetWorkflows();

        var accepted = GetAcceptedByTarget("in");
        var total = 0L;
        foreach (var ranges in accepted)
        {
            var count = 1L;
            foreach (var (_, length) in ranges)
            {
                count *= length;
            }
            total += count;
        }
        return total;

        List<List<Interval>> GetAcceptedByTarget(string target)
        {
            if (target == "A") return [[possible, possible, possible, possible]];
            if (target == "R") return [];
            return GetAcceptedByCondition(workflows[target].Conditions);
        }

        List<List<Interval>> GetAcceptedByCondition(ReadOnlySpan<Condition> conditions)
        {
            var c = conditions[0];
            if (c.LessThan == null) return GetAcceptedByTarget(c.Target);
            var whenTrue = ApplyCondition(GetAcceptedByTarget(c.Target), c);
            var whenFalse = ApplyCondition(GetAcceptedByCondition(conditions[1..]), c.Invert());
            whenTrue.AddRange(whenFalse);
            return whenTrue;
        }

        List<List<Interval>> ApplyCondition(List<List<Interval>> accept, Condition c)
        {
            var which = "xmas".IndexOf(c.Which);
            foreach (var ranges in accept)
            {
                var interval = ranges[which];
                var (start, last) = (interval.Start, interval.Last);
                if (c.LessThan == true)
                {
                    last = Math.Min(last, c.Value - 1);
                }
                else
                {
                    start = Math.Max(start, c.Value + 1);
                }
                if (start > last) continue;
                ranges[which] = Interval.RangeInclusive(start, last);
            }
            return accept;
        }
    }
}