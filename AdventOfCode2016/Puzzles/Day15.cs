using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day15 : Puzzle
{
    private (List<int>, List<int>) Config => Input.Extract<(int, int)>(@".+? has (\d+).+?position (\d+).").Separate();
    
    public override void PartOne()
    {
        var (m, a) = Config;
        var time = a.Select((ai, i) => -ai - i - 1).ChineseRemainder(m);
        WriteLn(time);
    }

    public override void PartTwo()
    {
        var (m, a) = Config;
        a.Add(0);
        m.Add(11);
        var time = a.Select((ai, i) => -ai - i - 1).ChineseRemainder(m);
        WriteLn(time);
    }
}