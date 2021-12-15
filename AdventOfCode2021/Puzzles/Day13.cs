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
        foreach (var line in lines)
        {
            var bounds = Grid.Bounds;
            var inst = line[11..];
            var pos = inst[2..].AsInt();
            if (inst[0] == 'x')
            {
                var area = new Rect(..pos, bounds.YRange);
                var window = Grid.View(area);
                window.FlipH = true;
                window.OffsetX = pos + 1;
                window.OverlayTransformed(Bools.Or);
                Grid.ClipTo(area);
            }
            else
            {
                var area = new Rect(bounds.XRange, ..pos);
                var window = Grid.View(area);
                window.FlipV = true;
                window.OffsetY = pos + 1;
                window.OverlayTransformed(Bools.Or);
                Grid.ClipTo(area);
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