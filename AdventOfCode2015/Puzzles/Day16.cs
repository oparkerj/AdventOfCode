using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day16 : Puzzle
{
    public Dictionary<string, int> Target = new();

    public Day16()
    {
        Target["children"] = 3;
        Target["cats"] = 7;
        Target["samoyeds"] = 2;
        Target["pomeranians"] = 3;
        Target["akitas"] = 0;
        Target["vizslas"] = 0;
        Target["goldfish"] = 5;
        Target["trees"] = 3;
        Target["cars"] = 2;
        Target["perfumes"] = 1;
    }

    public bool Fits(string s)
    {
        var data = s.After(':').Split(',', StringSplitOptions.TrimEntries).ReadKeys(a => a, int.Parse);
        foreach (var (key, _) in data)
        {
            if (Part == 2)
            {
                if (key is "cats" or "trees")
                {
                    if (data[key] <= Target[key]) return false;
                }
                else if (key is "pomeranians" or "goldfish")
                {
                    if (data[key] >= Target[key]) return false;
                }
                else if (data[key] != Target[key]) return false;
            }
            else if (data[key] != Target[key]) return false;
        }
        return true;
    }

    public override void PartOne()
    {
        foreach (var s in Input.Where(Fits))
        {
            WriteLn(s.Extract<int>(Patterns.Int));
        }
    }
}