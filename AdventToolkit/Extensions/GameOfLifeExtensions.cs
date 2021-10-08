using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Solvers;
using AdventToolkit.Utilities.Arithmetic;

namespace AdventToolkit.Extensions
{
    public static class GameOfLifeExtensions
    {
        public static int CountState<TLoc, TState>(this GameOfLife<TLoc, TState> game, TState state)
        {
            return game.CountValues(state);
        }
        
        public static int CountActive<T>(this GameOfLife<T, bool> game)
        {
            return game.CountValues(true);
        }

        public static void ReadFrom<TLoc, TState>(this GameOfLife<TLoc, TState> game, AlignedSpace<TLoc, TState> space)
        {
            foreach (var (pos, value) in space)
            {
                game[pos] = value;
            }
        }

        public static TSpace ToSpace<TSpace, TPos, TVal>(this GameOfLife<TPos, TVal> game)
            where TSpace : AlignedSpace<TPos, TVal>, new()
        {
            return game.ToSpace(() => new TSpace());
        }
        
        public static TSpace ToSpace<TSpace, TPos, TVal>(this GameOfLife<TPos, TVal> game, Func<TSpace> cons)
            where TSpace : AlignedSpace<TPos, TVal>
        {
            return game.CopyTo(cons());
        }

        public static TSpace CopyTo<TSpace, TPos, TVal>(this GameOfLife<TPos, TVal> game, TSpace space)
            where TSpace : AlignedSpace<TPos, TVal>
        {
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