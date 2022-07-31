using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2015.Puzzles;

public class Day19 : Puzzle
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
            select current.ReplaceAt(i, key, s);
    }

    public override void PartOne()
    {
        ReadInput();
        WriteLn(Replacements(AllGroups[1][0]).Distinct().Count());
    }

    public override void PartTwo()
    {
        ReadInput();
        var path = new Dijkstra<string>(Replacements);
        path.Heuristic = s => s.Length;
        var result = path.ComputeFind(AllGroups[1][0], "e");
        WriteLn(result);
    }
}