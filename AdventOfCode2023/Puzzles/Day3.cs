using AdventToolkit;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day3 : Puzzle<int>
{
    public override int PartOne()
    {
        var grid = Input.ToGrid();
        var numbers = new HashSet<Pos>();
        foreach (var (pos, value) in grid)
        {
            if (char.IsDigit(value) || value == '.') continue;
            numbers.UnionWith(grid.AroundWhere(pos, char.IsDigit).Select(grid.FindNumberBeginning));
        }
        return numbers.Select(grid.Read<int>).Sum();
    }

    public override int PartTwo()
    {
        var grid = Input.ToGrid();
        var sum = 0;
        foreach (var (pos, value) in grid)
        {
            if (value != '*') continue;
            var numbers = grid.AroundWhere(pos, char.IsDigit).Select(grid.FindNumberBeginning).ToHashSet();
            if (numbers.Count != 2) continue;
            sum += numbers.Select(grid.Read<int>).Product();
        }
        return sum;
    }
}