using System;
using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;

namespace AdventToolkit.Utilities
{
    public interface ITransformer<T, in TC>
    {
        T Transform(T pos, TC context);

        void PostTransform(TC context);
    }

    public static class TransformerTools
    {
        public static T GetTransformed<TPos, TVal, T>(this ITransformer<TPos, T> transformer, T space)
            where T : AlignedSpace<TPos, TVal>, new()
        {
            return transformer.GetTransformed<TPos, TVal, T>(space, () => new T());
        }
        
        public static T GetTransformed<TPos, TVal, T>(this ITransformer<TPos, T> transformer, T space, Func<T> cons)
            where T : AlignedSpace<TPos, TVal>
        {
            var s = cons();
            foreach (var (pos, value) in space)
            {
                s[transformer.Transform(pos, space)] = value;
            }
            transformer.PostTransform(s);
            return s;
        }

        // Apply a transformation to a space in-place
        // This method performs the transformation with side-effects disabled.
        public static void ApplyTo<TPos, TVal, T>(this ITransformer<TPos, T> transformer, T space)
            where T : AlignedSpace<TPos, TVal>
        {
            var pairs = space.ToList();
            space.Clear();
            if (space is Grid<TVal> g1) g1.FitBounds = false;
            foreach (var (pos, value) in pairs)
            {
                space[transformer.Transform(pos, space)] = value;
            }
            transformer.PostTransform(space);
            if (space is Grid<TVal> g2) g2.FitBounds = true;
        }

        public static void ApplyTo<T>(this ITransformer<Pos, Grid<T>> transformer, Grid<T> grid)
        {
            transformer.ApplyTo<Pos, T, Grid<T>>(grid);
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

    public class SimpleGridTransformer<T> : ITransformer<Pos, Grid<T>>
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

        public static ITransformer<Pos, Grid<T>> Translate(int x, int y) => new SimpleGridTransformer<T>((_, p) => p + new Pos(x, y), grid =>
        {
            grid.Bounds.MinXFixed += x;
            grid.Bounds.MinYFixed += y;
        });

        public static ITransformer<Pos, Grid<T>> ToFirstQuadrant() => new SimpleGridTransformer<T>((grid, p) => p - grid.Bounds.Min, grid =>
        {
            grid.Bounds.MinXFixed = 0;
            grid.Bounds.MinYFixed = 0;
        });

        public static ITransformer<Pos, Grid<T>> FlipH => new SimpleGridTransformer<T>((grid, p) =>
        {
            var d = grid.Bounds.MidX - p.X;
            return new Pos((int) (p.X + d * 2), p.Y);
        });

        public static ITransformer<Pos, Grid<T>> FlipV => new SimpleGridTransformer<T>((grid, p) =>
        {
            var d = grid.Bounds.MidY - p.Y;
            return new Pos(p.X, (int) (p.Y + d * 2));
        });

        public static ITransformer<Pos, Grid<T>> FlipSym => new SimpleGridTransformer<T>((_, p) => p.Flip());

        public static ITransformer<Pos, Grid<T>> RotateRight => new SimpleGridTransformer<T>((_, p) => p.Clockwise(), grid =>
        {
            var min = grid.Bounds.Min;
            var max = grid.Bounds.Max;
            grid.Bounds.Update(min.Clockwise(), max.Clockwise());
        });

        public static ITransformer<Pos, Grid<T>> RotateLeft => new SimpleGridTransformer<T>((_, p) => p.CounterClockwise(), grid =>
        {
            var min = grid.Bounds.Min;
            var max = grid.Bounds.Max;
            grid.Bounds.Update(min.CounterClockwise(), max.CounterClockwise());
        });
    }
}