using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using RegExtract;

namespace AdventOfCode2015.Puzzles;

public class Day6 : Puzzle
{
    public Grid<int> Lights = new();

    public void Turn(Rect rect, bool on)
    {
        foreach (var pos in rect)
        {
            if (Part == 2) Lights[pos] = Math.Max(Lights[pos] + (on ? 1 : -1), 0);
            else Lights[pos] = on.AsInt();
        }
    }

    public void Toggle(Rect rect)
    {
        foreach (var pos in rect)
        {
            if (Part == 2) Lights[pos] += 2;
            else Lights[pos] = 1 - Lights[pos];
        }
    }

    public override void PartOne()
    {
        var plan = ExtractionPlan<(int, int, int, int)>.CreatePlan(Patterns.Int4);
        foreach (var s in Input)
        {
            var rect = ((Pos4D) plan.Extract(s)).ToRectCorners();
            if (s.StartsWith("turn on")) Turn(rect, true);
            else if (s.StartsWith("turn off")) Turn(rect, false);
            else Toggle(rect);
        }
        WriteLn(Lights.Values.Count(1));
    }

    public override void PartTwo()
    {
        Run(PartOne);
        WriteLn(Lights.Values.Sum());
    }
}