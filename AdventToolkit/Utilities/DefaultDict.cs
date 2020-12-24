using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class DefaultDict<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
        {
            get => TryGetValue(key, out var t) ? t : default;
            set => base[key] = value;
        }
    }
}