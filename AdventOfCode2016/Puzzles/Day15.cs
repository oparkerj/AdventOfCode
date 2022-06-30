using AdventToolkit;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day15 : Puzzle
{
    public override void PartOne()
    {
        var disks = Input.Extract<(int, int)>(@".+? has (\d+).+?position (\d+).");

        var (m, a) = disks.Separate();
        var time = a.Select((ai, i) => -ai - i - 1).ChineseRemainder(m);
        WriteLn(time);
    }

    public override void PartTwo()
    {
        var disks = Input.Extract<(int, int)>(@".+? has (\d+).+?position (\d+).");

        var (m, a) = disks.Separate();
        a.Add(0);
        m.Add(11);
        var time = a.Select((ai, i) => -ai - i - 1).ChineseRemainder(m);
        WriteLn(time);
    }
}