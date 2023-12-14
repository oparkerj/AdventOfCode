using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day13 : Puzzle<int>
{
    public override int PartOne()
    {
        var tolerance = Part == 2 ? 1 : 0;
        return AllGroups.Select(s => s.ToGrid()).Select(Summarize).Sum();

        int Summarize(Grid<char> grid)
        {
            return CheckHorizontal(grid) + 100 * CheckVertical(grid);
        }
        
        int CheckHorizontal(Grid<char> grid)
        {
            var range = grid.Bounds.XRange;
            foreach (var start in range[..^1])
            {
                var found = true;
                var differences = 0;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    var count = grid.Col(first).Zip(grid.Col(last), (a, b) => a == b).Count(false);
                    differences += count;
                    if (differences > tolerance)
                    {
                        found = false;
                        break;
                    }
                }
                if (found && differences == tolerance) return start + 1;
            }
            return 0;
        }

        int CheckVertical(Grid<char> grid)
        {
            var range = grid.Bounds.YRange;
            foreach (var start in range[..^1])
            {
                var found = true;
                var differences = 0;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    var count = grid.Row(first).Zip(grid.Row(last), (a, b) => a == b).Count(false);
                    differences += count;
                    if (differences > tolerance)
                    {
                        found = false;
                        break;
                    }
                }
                if (found && differences == tolerance) return -start;
            }
            return 0;
        }
    }
}