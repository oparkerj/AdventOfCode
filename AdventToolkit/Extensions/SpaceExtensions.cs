using System.Collections.Generic;
using AdventToolkit.Utilities;

namespace AdventToolkit.Extensions
{
    public static class SpaceExtensions
    {
        public static bool ContainsValue<TPos, TVal>(this AlignedSpace<TPos, TVal> space, TVal val)
        {
            return space.Points.ContainsValue(val);
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
    }
}