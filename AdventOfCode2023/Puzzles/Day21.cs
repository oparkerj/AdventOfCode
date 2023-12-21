using System.Numerics;
using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Collections;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day21 : Puzzle<int, BigInteger>
{
    public override int PartOne()
    {
        var grid = Input.ToGrid();

        var current = new HashSet<Pos>();
        var temp = new HashSet<Pos>();
        current.Add(grid.Find('S'));

        var result = 0;
        for (var i = 0; i < 64; i++)
        {
            temp.Clear();
            foreach (var pos in current)
            {
                foreach (var next in pos.Adjacent().Where(p => grid.Has(p) && grid[p] != '#'))
                {
                    temp.Add(next);
                }
            }
            (current, temp) = (temp, current);
        }

        return current.Count;
    }

    public override BigInteger PartTwo()
    {
        var grid = Input.ToGrid();

        var current = new DefaultDict<Pos, BigInteger>();
        var temp = new DefaultDict<Pos, BigInteger>();
        current[grid.Find('S')] = 1;
        
        // TODO figure this out (super duper wrong)
        for (var i = 0; i < 64; i++)
        {
            temp.Clear();
            foreach (var (pos, amount) in current)
            {
                foreach (var next in pos.Adjacent().Where(p => grid[p] != '#'))
                {
                    temp[next] += amount;
                }
            }
            (current, temp) = (temp, current);
        }

        return current.Values.Sum();
    }
}