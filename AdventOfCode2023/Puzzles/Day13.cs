using AdventToolkit;
using AdventToolkit.Collections.Space;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day13 : Puzzle<int>
{
    // TODO clean this up
    public override int PartOne()
    {
        return AllGroups.Select(s => s.ToGrid()).Select(Summarize).Sum();

        int Summarize(Grid<char> grid)
        {
            return CheckHorizontal(grid) + 100 * CheckVertical(grid);
        }

        int CheckHorizontal(Grid<char> grid)
        {
            var range = grid.Bounds.XRange;
            var possible = new List<int>(2);
            for (var i = range.Start; i < range.End - 1; i++)
            {
                if (grid.Col(i).SequenceEqual(grid.Col(i + 1)))
                {
                    possible.Add(i);
                }
            }
            foreach (var start in possible)
            {
                var found = true;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    if (!grid.Col(first).SequenceEqual(grid.Col(last)))
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return start + 1;
            }
            return 0;
        }

        int CheckVertical(Grid<char> grid)
        {
            var range = grid.Bounds.YRange;
            var possible = new List<int>(2);
            for (var i = range.Start; i < range.End - 1; i++)
            {
                if (grid.Row(i).SequenceEqual(grid.Row(i + 1)))
                {
                    possible.Add(i);
                }
            }
            foreach (var start in possible)
            {
                var found = true;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    if (!grid.Row(first).SequenceEqual(grid.Row(last)))
                    {
                        found = false;
                        break;
                    }
                }
                if (found) return -start;
            }
            return 0;
        }
    }

    public override int PartTwo()
    {
        return AllGroups.Select(s => s.ToGrid()).Select(Summarize).Sum();

        int Summarize(Grid<char> grid)
        {
            return CheckHorizontal(grid) + 100 * CheckVertical(grid);
        }

        int CheckHorizontal(Grid<char> grid)
        {
            var range = grid.Bounds.XRange;
            foreach (var start in range)
            {
                var found = true;
                var differences = 0;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    var count = grid.Col(first).Zip(grid.Col(last), (a, b) => a == b).Count(false);
                    differences += count;
                    if (differences > 1)
                    {
                        found = false;
                        break;
                    }
                }
                if (found && differences == 1) return start + 1;
            }
            return 0;
        }

        int CheckVertical(Grid<char> grid)
        {
            var range = grid.Bounds.YRange;
            foreach (var start in range)
            {
                var found = true;
                var differences = 0;
                for (int first = start, last = start + 1; first >= range.Start && last < range.End; first--, last++)
                {
                    var count = grid.Row(first).Zip(grid.Row(last), (a, b) => a == b).Count(false);
                    differences += count;
                    if (differences > 1)
                    {
                        found = false;
                        break;
                    }
                }
                if (found && differences == 1) return -start;
            }
            return 0;
        }
    }
}