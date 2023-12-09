using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2023.Puzzles;

public class Day8 : Puzzle<int, long>
{
    public readonly string Directions;

    public Day8()
    {
        Directions = AllGroups[0][0];
    }

    public class Step
    {
        public string Node;
        public int Count;
    }

    public void NextStep(Step step, Dictionary<string, (string Left, string Right)> nodes)
    {
        var dir = Directions[step.Count];
        step.Node = dir == 'L' ? nodes[step.Node].Left : nodes[step.Node].Right;
        if (++step.Count >= Directions.Length)
        {
            step.Count = 0;
        }
    }
    
    public override int PartOne()
    {
        // TODO make a better input parser
        var nodes = AllGroups[1].Extract<KeyValuePair<string, string>>(@"(\w+) = \((\w+, \w+)\)")
            .Select(pair => new KeyValuePair<string, (string, string)>(pair.Key, pair.Value.SingleSplit(", ")))
            .ToDictionary();

        var step = new Step {Node = "AAA"};
        var total = 0;
        while (step.Node != "ZZZ")
        {
            NextStep(step, nodes);
            total++;
        }
        return total;
    }

    public override long PartTwo()
    {
        var nodes = AllGroups[1].Extract<KeyValuePair<string, string>>(@"(\w+) = \((\w+, \w+)\)")
            .Select(pair => new KeyValuePair<string, (string, string)>(pair.Key, pair.Value.SingleSplit(", ")))
            .ToDictionary();

        var cycles = new List<long>();
        foreach (var start in nodes.Keys.Where(s => s.EndsWith('A')))
        {
            var step = new Step {Node = start};
            // TODO this could be improved with a self-state version
            var (_, cycle) = Algorithms.FindCyclePeriod(step, s => (s.Node, s.Count), s => NextStep(s, nodes));
            cycles.Add(cycle);
        }

        return cycles.Aggregate(Num.Lcm);
    }
}