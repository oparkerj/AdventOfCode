using AdventToolkit;
using AdventToolkit.Collections;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

public class Day13 : Puzzle<int>
{
    public override int PartOne()
    {
        var tolerance = Part == 2 ? 1 : 0;
        return AllGroups.Select(s => s.ToGrid()).Sum(grid =>
        {
            // Horizontal symmetry + 100 * vertical symmetry
            return CheckLine(grid.Bounds.XRange, grid.Col, i => i + 1) + 100 * CheckLine(grid.Bounds.YRange, grid.Row, i => -i);

            // Range to check
            // Getter for line
            // How to fix the line number if we found the answer
            int CheckLine(Interval range, Func<int, IEnumerable<char>> get, Func<int, int> fix)
            {
                foreach (var start in range[..^1])
                {
                    var totalDiff = 0;
                    for (var first = start; first >= range.Start; first--)
                    {
                        var last = start * 2 - first + 1;
                        if (last >= range.End) break;
                        totalDiff += get(first).Zip(get(last)).Count(tuple => tuple.First != tuple.Second);
                        if (totalDiff > tolerance) break;
                    }
                    if (totalDiff == tolerance)
                    {
                        return fix(start);
                    }
                }
                return 0;
            }
        });
    }
}