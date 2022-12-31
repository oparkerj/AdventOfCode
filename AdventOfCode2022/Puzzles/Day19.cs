using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using RegExtract;

namespace AdventOfCode2022.Puzzles;

public class Day19 : Puzzle<int>
{
    public List<(Pos4D Need, Pos4D Make)> Costs;
    public IComparer<Pos4D> Comparer;

    public Resource Value(string name) => Enum.Parse<Resource>(name, true);

    public int RunBlueprint(string bp, int time)
    {
        var costs = bp.After(":").Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Costs = new List<(Pos4D Need, Pos4D Make)>();
        Comparer = Comparing<Pos4D>.By(p => p[0])
            .ThenBy(p => p[1])
            .ThenBy(p => p[2])
            .ThenBy(p => p[3]);

        foreach (var (component, parts) in costs.Extract<(string, List<(int, string)>)>(@"Each (\w+) robot costs ((\d+) (\w+)(?: and )?)+"))
        {
            var make = Pos4D.Index((int) Value(component));
            var cost = Pos4D.Zero;
            foreach (var (amount, part) in parts)
            {
                cost += Pos4D.Index((int) Value(part)) * amount;
            }
            Costs.Add((cost, make));
        }
        Costs.Add((Pos4D.Zero, Pos4D.Zero));

        return MaxGeodes(time);
    }

    public int MaxGeodes(int timeLeft)
    {
        var states = new List<(Pos4D Bots, Pos4D Have)>();
        var next = new List<(Pos4D Bots, Pos4D Have)>();
        states.Add(((0, 0, 0, 1), (0, 0, 0, 0)));
        for (var i = 0; i < timeLeft; i++)
        {
            next.Clear();
            foreach (var (bots, have) in states)
            {
                foreach (var (need, make) in Costs)
                {
                    if (need <= have) next.Add((bots + make, have + bots - need));
                }
            }
            states.Clear();
            states.AddRange(next.OrderByDescending(tuple => tuple.Have + tuple.Bots * 2, Comparer).Take(1000));
        }
        return states.Max(tuple => tuple.Have[0]);
    }

    public override int PartOne()
    {
        return Input.Select((bp, i) => (i + 1) * RunBlueprint(bp, 24)).Sum();
    }

    public override int PartTwo()
    {
        return Input.Take(3).Select(s => RunBlueprint(s, 32)).Product();
    }

    public enum Resource
    {
        Geode = 0,
        Obsidian,
        Clay,
        Ore,
    }
}