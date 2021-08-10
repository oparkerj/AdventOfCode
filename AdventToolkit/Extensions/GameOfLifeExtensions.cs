using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities;
using AdventToolkit.Utilities.Arithmetic;

namespace AdventToolkit.Extensions
{
    public static class GameOfLifeExtensions
    {
        public static int CountActive<T>(this GameOfLife<T, bool> game)
        {
            return game.Count(pair => pair.Value);
        }

        public static TSpace ToSpace<TSpace, TPos, TVal>(this GameOfLife<TPos, TVal> game)
            where TSpace : AlignedSpace<TPos, TVal>, new()
        {
            return game.ToSpace(() => new TSpace());
        }
        
        public static TSpace ToSpace<TSpace, TPos, TVal>(this GameOfLife<TPos, TVal> game, Func<TSpace> cons)
            where TSpace : AlignedSpace<TPos, TVal>
        {
            var space = cons();
            foreach (var (pos, val) in game)
            {
                space[pos] = val;
            }
            return space;
        }

        public static IEnumerable<TPos> ToTrace<TPos, TVal>(this IEnumerable<TPos> neighbors,
            TPos center,
            GameOfLife<TPos, TVal> game,
            Func<TPos, TVal, bool> hitCondition)
            where TPos : IAdd<TPos>
        {
            return neighbors.Select(dir => center.Trace(dir, pos => !game.Has(pos) || hitCondition(pos, game[pos])));
        }
    }
}