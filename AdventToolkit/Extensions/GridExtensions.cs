using System.Collections.Generic;
using AdventToolkit.Collections;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Utilities;
using MoreLinq.Extensions;

namespace AdventToolkit.Extensions
{
    public static class GridExtensions
    {
        public static TGrid ToGrid<T, TGrid>(this IEnumerable<IEnumerable<T>> source, bool decreaseY = true)
            where TGrid : Grid<T>, new()
        {
            var grid = new TGrid();
            var y = 0;
            foreach (var row in source)
            {
                var x = 0;
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
    }
}