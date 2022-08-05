using AdventToolkit;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2015.Puzzles;

public class Day5 : Puzzle<int>
{
    public bool IsNice(string s)
    {
        var vowels = s.Frequencies().WhereKey(c => "aeiou".Contains(c)).Values().Sum();
        if (vowels < 3) return false;
        if (!s.RunLengthEncode().Any(pair => pair.Value >= 2)) return false;
        return !s.Contains("ab") && !s.Contains("cd") && !s.Contains("pq") && !s.Contains("xy");
    }
    
    public override int PartOne()
    {
        return Input.Count(IsNice);
    }

    public bool IsNice2(string s)
    {
        var pair = s.SlideView(2)
            .StrEach()
            .Any(str => s.IndexOf(str) is var i && i < s.Length - 2 && s.IndexOf(str, i + 2) > -1);
        return pair && s.SlideView(3).Any(view => view[0] == view[2]);
    }

    public override int PartTwo()
    {
        return Input.Count(IsNice2);
    }
}