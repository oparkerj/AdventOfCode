using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;

namespace AdventToolkit.Extensions;

public static class SpaceExtensions
{
    public static bool ContainsValue<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TVal val)
    {
        return space.HasValue(val);
    }

    public delegate void SpaceFilter<TPos, TVal>(ref TPos pos, ref TVal val);

    public static IEnumerable<(TPos, TVal)> Select<TPos, TVal>(this AlignedSpace<TPos, TVal> space, SpaceFilter<TPos, TVal> filter)
    {
        foreach (var (pos, val) in space)
        {
            var p = pos;
            var v = val;
            filter?.Invoke(ref p, ref v);
            yield return (p, v);
        }
    }

    public static IEnumerable<TVal> GetFrom<TPos, TVal>(this IEnumerable<TPos> source, AlignedSpace<TPos, TVal> space)
    {
        return source.Select(pos => space[pos]);
    }

    public static IEnumerable<TPos> Neighbors<TPos, TVal>(this TPos pos, AlignedSpace<TPos, TVal> space)
    {
        return space.GetNeighbors(pos);
    }

    public static IEnumerable<TVal> NeighborValues<TPos, TVal>(this TPos pos, AlignedSpace<TPos, TVal> space)
    {
        return space.GetNeighbors(pos).GetFrom(space);
    }

    public static IEnumerable<TVal> GetNeighborValues<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos pos)
    {
        return space.GetNeighbors(pos).GetFrom(space);
    }

    public static IEnumerable<(TPos, TVal)> GetNeighborsAndValues<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TPos pos)
    {
        return space.GetNeighbors(pos).Select(p => (p, space[p]));
    }

    public static TPos Find<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TVal value)
    {
        return space.WhereValue(value).First().Key;
    }

    public static bool Find<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TVal value, out TPos pos)
    {
        return space.WhereValue(value).Keys().First(out pos);
    }

    public static void ApplyValues<TPos, TVal>(this AlignedSpace<TPos, TVal> space, IEnumerable<TPos> positions, Func<TPos, TVal> func)
    {
        foreach (var pos in positions)
        {
            space[pos] = func(pos);
        }
    }
}