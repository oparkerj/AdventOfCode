using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day11 : Puzzle<long>
{
    public override long PartOne()
    {
        var emptySpace = Part == 2 ? 1_000_000 : 2;
        
        var grid = Input.ToGrid();
        var expanded = new HashSet<Pos>();
        expanded.UnionWith(grid.WhereValue('#').Keys());
        var temp = new HashSet<Pos>();
        
        foreach (var x in grid.Bounds.XRange.Reverse())
        {
            if (grid.Col(x).All(c => c != '#'))
            {
                temp.Clear();
                temp.UnionWith(expanded.Select(pos => pos.X > x ? new Pos(pos.X + emptySpace - 1, pos.Y) : pos));
                (expanded, temp) = (temp, expanded);
            }
        }
        foreach (var y in grid.Bounds.YRange.Reverse())
        {
            if (grid.Row(y).All(c => c != '#'))
            {
                temp.Clear();
                temp.UnionWith(expanded.Select(pos => pos.Y > y ? new Pos(pos.X, pos.Y + emptySpace - 1) : pos));
                (expanded, temp) = (temp, expanded);
            }
        }
        
        return expanded.ToList().Pairs().Select(tuple => tuple.Item1.MDist(tuple.Item2)).Longs().Sum();
    }
}