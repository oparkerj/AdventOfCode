using System.Collections.Generic;

namespace AdventToolkit.Utilities
{
    public class DefaultDict<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public DefaultDict() { }
        
        public DefaultDict(IDictionary<TKey, TValue> dict) : base(dict) { }

        public TValue DefaultValue = default;

        public new TValue this[TKey key]
        {
            get => TryGetValue(key, out var t) ? t : DefaultValue;
            set => base[key] = value;
        }
    }
}