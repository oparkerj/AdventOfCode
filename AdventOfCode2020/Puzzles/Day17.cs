using System.Collections.Generic;
using System.Linq;
using AdventToolkit;
using AdventToolkit.Data;
using AdventToolkit.Extensions;

namespace AdventOfCode2020.Puzzles
{
    public class Day17 : Puzzle
    {
        public Day17()
        {
            Part = 2;
        }

        public override void PartOne()
        {
            var width = Input[0].Length;
            var height = Input.Length;
            var grid = new DefaultDict<(int, int, int), bool>();
            var temp = new DefaultDict<(int, int, int), bool>();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    grid[(i, j, 0)] = Input[j][i] == '#';
                }
            }
            
            for (var i = 0; i < 6; i++)
            {
                temp.Clear();
                var consider = grid.SelectMany(pair => pair.Key.Around()).Concat(grid.Keys).ToHashSet();
                foreach (var pos in consider)
                {
                    if (grid[pos])
                    {
                        if (pos.Around().Count(p => grid[p]) is 2 or 3) temp[pos] = true;
                        else temp[pos] = false;
                    }
                    else
                    {
                        if (pos.Around().Count(p => grid[p]) == 3) temp[pos] = true;
                        else temp[pos] = false;
                    }
                }
                Data.Swap(ref grid, ref temp);
            }
            WriteLn(grid.Count(pair => pair.Value));
        }

        public override void PartTwo()
        {
            var width = Input[0].Length;
            var height = Input.Length;
            var grid = new DefaultDict<(int, int, int, int), bool>();
            var temp = new DefaultDict<(int, int, int, int), bool>();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    grid[(i, j, 0, 0)] = Input[j][i] == '#';
                }
            }
            
            for (var i = 0; i < 6; i++)
            {
                temp.Clear();
                var consider = grid.SelectMany(pair => pair.Key.Around()).Concat(grid.Keys).ToHashSet();
                foreach (var pos in consider)
                {
                    if (grid.GetValueOrDefault(pos))
                    {
                        if (pos.Around().Count(p => grid[p]) is 2 or 3) temp[pos] = true;
                        else temp[pos] = false;
                    }
                    else
                    {
                        if (pos.Around().Count(p => grid[p]) == 3) temp[pos] = true;
                        else temp[pos] = false;
                    }
                }
                Data.Swap(ref grid, ref temp);
            }
            WriteLn(grid.Count(pair => pair.Value));
        }
    }
}