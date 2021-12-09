using System;
using System.Linq;
using System.Numerics;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;
using MoreLinq;

namespace AdventOfCode2017.Puzzles;

public class Day14 : Puzzle
{
    public Day14()
    {
        Part = 2;
    }

    public override void PartOne()
    {
        var key = InputLine;
        var result = Enumerable.Range(0, 128)
            .Select(i => $"{key}-{i}")
            .Select(Day10.KnotHash)
            .SelectMany(s => s.Batch(8).ToStrings())
            .Select(s => Convert.ToUInt32(s, 16))
            .Select(BitOperations.PopCount)
            .Sum();
        WriteLn(result);
    }

    public override void PartTwo()
    {
        var key = InputLine;
        var data = Enumerable.Range(0, 128)
            .Select(i => $"{key}-{i}")
            .Select(Day10.KnotHash)
            .SelectMany(s => s.Batch(8).ToStrings())
            .SelectMany(s => Convert.ToUInt32(s, 16).Bits())
            .ToGridRows(128, true);

        var currentGroup = 0;
        var regions = new Grid<int>();
        foreach (var pos in data.Positions)
        {
            if (regions.Has(pos) || !data[pos]) continue;
            currentGroup++;
            regions[pos] = currentGroup;
            foreach (var inside in data.Bfs(pos, p => data[p], _ => true))
            {
                regions[inside] = currentGroup;
            }
        }
        WriteLn(currentGroup);
    }
}