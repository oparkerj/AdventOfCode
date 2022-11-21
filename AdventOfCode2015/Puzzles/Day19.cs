using AdventToolkit;
using AdventToolkit.Extensions;

namespace AdventOfCode2015.Puzzles;

public class Day19 : Puzzle<int>
{
    public Dictionary<string, List<string>> Reactions = new();

    public void ReadInput()
    {
        foreach (var s in AllGroups[0])
        {
            var (part, result) = s.SingleSplit(" => ");
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
        var target = AllGroups[1][0].AsSpan();
        var result = -1;

        for (var i = 0; i < target.Length - 1; i++)
        {
            var part = target.Slice(i, 2);
            if (part is "Rn") continue;
            if (part is "Ar") continue;
            if (part[0] == 'Y') result--;
            else if (char.IsUpper(part[0])) result++;
        }
        if (char.IsUpper(target[^1])) result++;
        
        return result;
    }
}