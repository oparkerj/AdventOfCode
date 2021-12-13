using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;

namespace AdventOfCode2021.Puzzles;

public class Day13 : Puzzle
{
    public Grid<bool> Grid;

    public Day13()
    {
        Part = 2;

        Grid = new Grid<bool>();
        foreach (var pos in AllGroups[0].Select(Pos.Parse))
        {
            Grid[pos] = true;
        }
    }

    public void RunInstructions(IEnumerable<string> lines)
    {
        foreach (var s in lines)
        {
            var bounds = Grid.Bounds;
            var inst = s[11..];
            var pos = inst[2..].AsInt();
            if (inst[0] == 'x')
            {
                for (var i = 0; i < bounds.MaxX - pos; i++)
                {
                    foreach (var y in bounds.YRange)
                    {
                        if (Grid[pos + i + 1, y]) Grid[pos - i - 1, y] = true;
                    }
                }
                Grid.Clip(new Rect(Interval.Range(bounds.MinX, pos), bounds.YRange));
            }
            else
            {
                foreach (var x in bounds.XRange)
                {
                    for (var i = 0; i < bounds.MaxY - pos; i++)
                    {
                        if (Grid[x, pos + i + 1]) Grid[x, pos - i - 1] = true;
                    }
                }
                Grid.Clip(new Rect(bounds.XRange, Interval.Range(bounds.MinY, pos)));
            }
        }
    }

    public override void PartOne()
    {
        RunInstructions(AllGroups[1].Take(1));
        WriteLn(Grid.Values.Count(true));
    }

    public override void PartTwo()
    {
        RunInstructions(AllGroups[1]);
        var code = Grid.ToArray().Stringify(b => b ? '#' : ' ', true);
        WriteLn(code);
    }
}