using System;
using System.Runtime.CompilerServices;

namespace AdventToolkit.Extensions
{
    public static class ValueTupleExtensions
    {
        public static T[] ToArray<T>(this ITuple tuple)
        {
            var result = new T[tuple.Length];
            for (var i = 0; i < result.Length; i++)
            {
                if (tuple[i] is T t) result[i] = t;
                else throw new ArgumentException("Tuple type does not match.");
            }
            return result;
        }
    }
}