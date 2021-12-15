using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Utilities;
using MoreLinq.Extensions;

namespace AdventToolkit.Extensions;

public static class GridExtensions
{
    public static TGrid ToGrid<T, TGrid>(this IEnumerable<IEnumerable<T>> source, TGrid grid, Pos offset = default, bool decreaseY = true)
        where TGrid : GridBase<T>
    {
        var y = offset.Y;
        foreach (var row in source)
        {
            var x = offset.X;
            foreach (var t in row)
            {
                grid[x, y] = t;
                x++;
            }
            if (decreaseY) y--;
            else y++;
        }
        return grid;
    }
        
    public static TGrid ToGrid<T, TGrid>(this IEnumerable<IEnumerable<T>> source, bool decreaseY = true)
        where TGrid : GridBase<T>, new()
    {
        return ToGrid(source, new TGrid(), default, decreaseY);
    }
        
    public static Grid<T> ToGrid<T>(this IEnumerable<IEnumerable<T>> source, bool decreaseY = true)
    {
        return source.ToGrid<T, Grid<T>>(decreaseY);
    }

    public static Grid<bool> ToGrid(this IEnumerable<Pos> source)
    {
        var grid = new Grid<bool>();
        foreach (var pos in source)
        {
            grid[pos] = true;
        }
        return grid;
    }

    public static Grid<T> ToGridRows<T>(this IEnumerable<T> source, int width, bool decreaseY = false)
    {
        var grid = new Grid<T>();
        foreach (var (y, row) in source.Batch(width).Index())
        {
            foreach (var (x, item) in row.Index())
            {
                grid[x, y] = item;
            }
        }
        if (decreaseY) SimpleGridTransformer<T>.FlipV.ApplyTo(grid);
        return grid;
    }

    public static FixedGrid<T> ToFixedGrid<T>(this IEnumerable<IEnumerable<T>> source, int width, int height)
    {
        return source.Select(row => row.Take(width)).Take(height).ToGrid(new FixedGrid<T>(width, height), decreaseY: false);
    }

    public static void BuildSummedArea(this Grid<int> grid)
    {
        foreach (var pos in grid.Bounds)
        {
            grid[pos] += grid[pos + Pos.Left] + grid[pos + Pos.Down] - grid[pos + Pos.Left + Pos.Down];
        }
    }

    public static int GetSummedArea(this Grid<int> grid, Rect rect)
    {
        var (min, max, diagMin, diagMax) = (
            rect.Min + new Pos(-1, -1),
            rect.Max,
            rect.DiagMin + Pos.Down,
            rect.DiagMax + Pos.Left);
        return grid[min] + grid[max] - grid[diagMin] - grid[diagMax];
    }

    public static Grid<T> Slice<T>(this Grid<T> grid, Interval x, Interval y, bool decreaseY = false, Grid<T> output = null)
    {
        output ??= new Grid<T>();
        var min = new Pos(x.Start, decreaseY ? y.Start + y.Length : y.Start);
        foreach (var i in x)
        {
            foreach (var j in y)
            {
                var pos = new Pos(i, j);
                output[pos - min] = grid[pos];
            }
        }
        return output;
    }

    public static Grid<T> Slice<T>(this Grid<T> grid, Pos offset, int sizeX, int sizeY, bool decreaseY = false, Grid<T> output = null)
    {
        var x = new Interval(offset.X, sizeX);
        var y = decreaseY ? new Interval(offset.Y - sizeY + 1, sizeY) : new Interval(offset.Y, sizeY);
        return Slice(grid, x, y, decreaseY, output);
    }

    public static GridView<T> View<T>(this GridBase<T> grid, Rect window) => new(grid, window);

    public static GridView<T> View<T>(this GridBase<T> grid, Interval xRange, Interval yRange)
    {
        return grid.View(new Rect(xRange, yRange));
    }

    public static void ClipTo<T>(this GridBase<T> grid, Rect window)
    {
        var clip = grid.Bounds.Intersection(window);
        foreach (var outside in grid.Positions.Where(pos => !clip.Contains(pos)).ToList())
        {
            grid.Remove(outside);
        }
        grid.Bounds = clip;
    }

    public static void KeepOnly<T>(this GridBase<T> grid, T value)
    {
        foreach (var pos in grid.WhereValue(t => !Equals(t, value)).Keys().ToList())
        {
            grid.Remove(pos);
        }
        grid.ResetBounds();
    }

    // TODO remove this once generic math feature is available
    public static Dijkstra<Pos, (Pos, int)> ToDijkstraWeights(this GridBase<int> grid, bool inBounds = true)
    {
        return new Dijkstra<Pos, (Pos, int)>
        {
            Neighbors = pos => inBounds ? grid.GetNeighbors(pos).Where(grid.Bounds.Contains).Select(p => (p, grid[p])) : grid.GetNeighborsAndValues(pos),
            Distance = tuple => tuple.Item2,
            Cell = (_, tuple) => tuple.Item1
        };
    }
}