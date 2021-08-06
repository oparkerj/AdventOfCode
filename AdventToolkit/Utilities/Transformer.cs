using System;
using System.Linq;

namespace AdventToolkit.Utilities
{
    public interface ITransformer<T, TC>
    {
        Pos Transform(Pos p, TC context);

        void PostTransform(TC context);
    }

    public interface ITransformer<T> : ITransformer<T, int> { }

    public static class TransformerTools
    {
        public static Grid<T> GetTransformed<T>(this ITransformer<T, Grid<T>> transformer, Grid<T> grid)
        {
            var g = new Grid<T>();
            foreach (var pair in grid)
            {
                g[transformer.Transform(pair.Key, grid)] = pair.Value;
            }
            grid.ResetBounds();
            return g;
        }

        public static void ApplyTo<T>(this ITransformer<T, Grid<T>> transformer, Grid<T> grid)
        {
            var pairs = grid.ToList();
            grid.Clear();
            foreach (var (pos, value) in pairs)
            {
                grid[transformer.Transform(pos, grid), false] = value;
            }
            transformer.PostTransform(grid);
        }

        public static bool TryAllOrientations<T>(this Grid<T> grid, Func<Grid<T>, bool> func)
        {
            var rotate = SimpleGridTransformer<T>.RotateRight;
            for (var i = 0; i < 4; i++)
            {
                rotate.ApplyTo(grid);
                if (func(grid)) return true;
            }
            SimpleGridTransformer<T>.FlipH.ApplyTo(grid);
            for (var i = 0; i < 4; i++)
            {
                rotate.ApplyTo(grid);
                if (func(grid)) return true;
            }
            SimpleGridTransformer<T>.FlipH.ApplyTo(grid);
            return false;
        }
    }

    public class SimpleGridTransformer<T> : ITransformer<T, Grid<T>>
    {
        public readonly Func<Grid<T>, Pos, Pos> Transformer;
        public readonly Action<Grid<T>> PostTransformer;

        public SimpleGridTransformer(Func<Grid<T>, Pos, Pos> transformer, Action<Grid<T>> postTransformer = null)
        {
            Transformer = transformer;
            PostTransformer = postTransformer;
        }

        public Pos Transform(Pos p, Grid<T> grid) => Transformer(grid, p);

        public void PostTransform(Grid<T> grid) => PostTransformer?.Invoke(grid);

        public static ITransformer<T, Grid<T>> Translate(int x, int y) => new SimpleGridTransformer<T>((_, p) => p + new Pos(x, y), grid =>
        {
            grid.Bounds.MinX += x;
            grid.Bounds.MinY += y;
            grid.Bounds.MaxX += x;
            grid.Bounds.MaxY += y;
        });

        public static ITransformer<T, Grid<T>> ToFirstQuadrant() => new SimpleGridTransformer<T>((grid, p) => p - grid.Bounds.Min, grid =>
        {
            grid.Bounds.MaxX -= grid.Bounds.MinX;
            grid.Bounds.MaxY -= grid.Bounds.MinY;
            grid.Bounds.MinX = 0;
            grid.Bounds.MinY = 0;
        });

        public static ITransformer<T, Grid<T>> FlipH => new SimpleGridTransformer<T>((grid, p) =>
        {
            var d = grid.Bounds.MidX - p.X;
            return new Pos((int) (p.X + d * 2), p.Y);
        });

        public static ITransformer<T, Grid<T>> FlipV => new SimpleGridTransformer<T>((grid, p) =>
        {
            var d = grid.Bounds.MidY - p.Y;
            return new Pos(p.X, (int) (p.Y + d * 2));
        });

        public static ITransformer<T, Grid<T>> FlipSym => new SimpleGridTransformer<T>((_, p) => p.Flip());

        public static ITransformer<T, Grid<T>> RotateRight => new SimpleGridTransformer<T>((_, p) => (p.Y, -p.X), grid =>
        {
            var mnx = grid.Bounds.MinX;
            var mxx = grid.Bounds.MaxX;
            grid.Bounds.MinX = grid.Bounds.MinY;
            grid.Bounds.MaxX = grid.Bounds.MaxY;
            grid.Bounds.MinY = -mxx;
            grid.Bounds.MaxY = -mnx;
        });

        public static ITransformer<T, Grid<T>> RotateLeft => new SimpleGridTransformer<T>((_, p) => (-p.Y, p.X), grid =>
        {
            var mnx = grid.Bounds.MinX;
            var mxx = grid.Bounds.MaxX;
            grid.Bounds.MinX = -grid.Bounds.MaxY;
            grid.Bounds.MaxX = -grid.Bounds.MinY;
            grid.Bounds.MinY = mnx;
            grid.Bounds.MaxY = mxx;
        });
    }
}