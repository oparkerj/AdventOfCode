using AdventToolkit;
using AdventToolkit.Attributes;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventOfCode2023.Puzzles;

[CopyResult]
public class Day16 : Puzzle<int>
{
    public Pos[] Bend(char c, Pos dir)
    {
        return (c, dir) switch
        {
            ('/', (1, 0)) => [Pos.Up],
            ('/', (-1, 0)) => [Pos.Down],
            ('/', (0, 1)) => [Pos.Right],
            ('/', (0, -1)) => [Pos.Left],
            ('\\', (1, 0)) => [Pos.Down],
            ('\\', (-1, 0)) => [Pos.Up],
            ('\\', (0, 1)) => [Pos.Left],
            ('\\', (0, -1)) => [Pos.Right],
            ('-', (0, 1)) => [Pos.Right, Pos.Left],
            ('-', (0, -1)) => [Pos.Right, Pos.Left],
            ('|', (1, 0)) => [Pos.Up, Pos.Down],
            ('|', (-1, 0)) => [Pos.Up, Pos.Down],
            _ => [dir]
        };
    }

    public int TryBeam(Grid<char> grid, Pos start, Pos startDir)
    {
        var beams = new Queue<(Pos, Pos)> {(start - startDir, startDir)};
        var energized = new HashSet<Pos>();
        var hits = new HashSet<(Pos, Pos)>();

        while (beams.Count > 0)
        {
            var (pos, dir) = beams.Dequeue();
            var hit = pos.Trace(dir, p => grid[p] != '.');
            // If already hit here before, stop tracing
            if (!hits.Add((hit, dir))) continue;
            energized.UnionWith(pos.EachBetween(hit));
            // If went off the grid, stop tracing
            if (!grid.Has(hit)) continue;
            // If still on the grid, add the hit location
            energized.Add(hit);
            beams.AddRange(Bend(grid[hit], dir).Select(nextDir => (hit, nextDir)));
        }

        return energized.Count;
    }
    
    public override int PartOne()
    {
        var grid = Input.ToGrid();
        return TryBeam(grid, grid.Bounds.DiagMinMax, Pos.Right);
    }

    public override int PartTwo()
    {
        var grid = Input.ToGrid();

        var (min, max) = grid.Bounds;
        KeyValuePair<Pos, Pos>[] starts = [
            ..grid.Bounds.XRange.Select(i => new Pos(i, min.Y)).With(Pos.Up),
            ..grid.Bounds.XRange.Select(i => new Pos(i, max.Y)).With(Pos.Down),
            ..grid.Bounds.YRange.Select(i => new Pos(min.X, i)).With(Pos.Right),
            ..grid.Bounds.YRange.Select(i => new Pos(max.X, i)).With(Pos.Left)
        ];

        return starts.Max(pair => TryBeam(grid, pair.Key, pair.Value));
    }
}