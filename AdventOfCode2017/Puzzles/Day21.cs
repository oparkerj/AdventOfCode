using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2017.Puzzles;

public class Day21 : Puzzle
{
    public Grid<bool> Image = new();
    private Grid<bool> _temp = new();

    public Dictionary<string, string> Rules = new();

    public Day21()
    {
        Image[1, 0] = true;
        Image[2, -1] = true;
        Image[..3, -2] = true;

        Part = 2;
    }

    public void LoadRules()
    {
        var grid = new Grid<bool>();
        foreach (var rule in Input)
        {
            var parts = rule.Split(" => ");
            FromKey(parts[0]).ToGrid(grid);
            grid.ForAllOrientations(g =>
            {
                var key = ToKey(g.ToArray());
                Rules[key] = parts[1];
            });
        }
    }

    public void Step()
    {
        var size = Image.Bounds.Width;
        var chunk = size % 2 == 0 ? 2 : 3;
        var len = size / chunk;
        var slice = new Grid<bool>();
        var temp = new bool[chunk, chunk];
        for (var i = 0; i < len; i++)
        {
            for (var j = 0; j < len; j++)
            {
                var corner = new Pos(i * chunk, -j * chunk);
                Image.Slice(corner, chunk, chunk, true, slice);
                var key = ToKey(slice.ToArray(temp));
                var outCorner = new Pos(i * (chunk + 1), -j * (chunk + 1));
                FromKey(Rules[key]).ToGrid(_temp, outCorner);
            }
        }
        Data.Swap(ref Image, ref _temp);
    }

    public IEnumerable<IEnumerable<bool>> FromKey(string key)
    {
        return key.Split('/').Select2D(c => c == '#');
    }

    public string ToKey(bool[,] grid)
    {
        return grid.Stringify(b => b ? '#' : '.', rowSep: "/");
    }

    public override void PartOne()
    {
        LoadRules();
        foreach (var _ in Enumerable.Range(0, 5))
        {
            Step();
        }
        var result = Image.CountValues(true);
        WriteLn(result);
    }

    public override void PartTwo()
    {
        LoadRules();
        foreach (var _ in Enumerable.Range(0, 18))
        {
            Step();
        }
        var result = Image.CountValues(true);
        WriteLn(result);
    }
}