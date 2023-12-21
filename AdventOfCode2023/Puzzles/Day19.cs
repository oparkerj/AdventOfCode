using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day19 : Puzzle<int>
{
    public record Part(int X, int M, int A, int S);

    public record Condition(Func<Part, int> Accessor, bool LessThan, int Value, string Target);

    public record Workflow(List<Condition> Conditions, string Default);

    public override int PartOne()
    {
        var parts = AllGroups[1].Extract<Part>(Patterns.Int4).ToList();

        var workflows = AllGroups[0].Select(s =>
        {
            var name = s.Before('{');
            var rules = s.Between('{', '}').Split(',').AsSpan();
            var @default = rules[^1];
            var conditions = new List<Condition>();
            foreach (var cond in rules[..^1])
            {
                Func<Part, int> accessor = cond[0] switch
                {
                    'x' => part => part.X,
                    'm' => part => part.M,
                    'a' => part => part.A,
                    's' => part => part.S,
                };
                var lt = cond[1] == '<';
                var value = cond[2..].Before(':').AsInt();
                var target = cond.After(':');
                conditions.Add(new Condition(accessor, lt, value, target));
            }
            return new KeyValuePair<string, Workflow>(name, new Workflow(conditions, @default));
        }).ToDictionary();

        return parts.Where(IsAccepted)
            .Select(part => part.X + part.M + part.A + part.S)
            .Sum();

        bool IsAccepted(Part part)
        {
            var workflow = "in";
            while (true)
            {
                workflow = GetNext(part, workflows[workflow]);
                if (workflow is "A") return true;
                if (workflow is "R") return false;
            }
        }

        string GetNext(Part part, Workflow workflow)
        {
            var (conditions, @default) = workflow;
            foreach (var (accessor, lt, value, target) in conditions)
            {
                var left = accessor(part);
                if ((lt && left < value) || (!lt && left > value))
                {
                    return target;
                }
            }
            return @default;
        }
    }
}