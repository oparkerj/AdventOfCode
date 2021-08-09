using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static IEnumerable<TKey> Keys<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.Select(pair => pair.Key);
        }
        
        public static IEnumerable<TValue> Values<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.Select(pair => pair.Value);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> WhereKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TKey key)
        {
            return source.Where(pair => Equals(pair.Key, key));
        }
        
        public static IEnumerable<KeyValuePair<TKey, TValue>> WhereValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, TValue value)
        {
            return source.Where(pair => Equals(pair.Value, value));
        }
    }
}