using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2015.Puzzles;

public class Day19 : Puzzle<int>
{
    public Dictionary<string, List<string>> Reactions = new();

    public void ReadInput()
    {
        foreach (var s in AllGroups[0])
        {
            var (part, result) = s.SingleSplit(" => ");
            if (Part == 2) Data.Swap(ref part, ref result);
            Reactions.GetOrNew(part).Add(result);
        }
    }

    public IEnumerable<string> Replacements(string current)
    {
        return from key in Reactions.Keys
            from i in current.IndicesOf(key, true)
            from s in Reactions[key]
            select current.ReplaceAt(i, key.Length, s);
    }

    public override int PartOne()
    {
        ReadInput();
        return Replacements(AllGroups[1][0]).Distinct().Count();
    }

    public override int PartTwo()
    {
        ReadInput();
        var path = new Dijkstra<string>(Replacements);
        path.Heuristic = s => s.Length;
        return path.ComputeFind(AllGroups[1][0], "e");
    }
}