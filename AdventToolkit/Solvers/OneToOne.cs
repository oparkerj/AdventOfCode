using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Solvers
{
    public class OneToOne<TKey, TValue>
    {
        private readonly HashSet<TValue> _values = new();
        private readonly Dictionary<TKey, HashSet<TValue>> _possible = new();

        public TValue Get(TKey key) => _possible[key].First();

        // Adds a key that starts with every possible value
        public void Add(TKey key)
        {
            if (_possible.ContainsKey(key)) return;
            _possible[key] = new HashSet<TValue>(_values);
        }

        public void AddKeys(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                Add(key);
            }
        }

        // Adds the specified values as a possible option for every key
        public void AddValue(TValue value)
        {
            _values.Add(value);
            foreach (var (_, options) in _possible)
            {
                options.Add(value);
            }
        }

        public void AddValues(IEnumerable<TValue> values)
        {
            foreach (var value in values)
            {
                AddValue(value);
            }
        }

        // Indicate that a set of values exists in some larger set of keys.
        // This will remove the values from being a possibility in any keys not given.
        public void ValuesPresentInKeys(IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            var keyList = keys.ToList();
            var valueList = values.ToList();
            foreach (var options in _possible.Keys.Except(keyList).Select(key => _possible[key]))
            {
                foreach (var value in valueList)
                {
                    options.Remove(value);
                }
            }
        }

        // Remove keys that have no possibilites
        public void RemoveExtra()
        {
            foreach (var key in _possible.WhereValue(options => options.Count == 0).Keys().ToList())
            {
                _possible.Remove(key);
            }
        }

        // Get a collection of values that are valid for each key, and remove any
        // other values
        public void ReduceWithValid(Func<TKey, IEnumerable<TValue>> keep)
        {
            foreach (var (key, options) in _possible)
            {
                var valid = keep(key).ToHashSet();
                options.RemoveWhere(value => !valid.Contains(value));
            }
        }

        // Get a collection of tests for each key, use that test to eliminate values for the option
        public void ReduceWithValid<TC>(Func<TKey, IEnumerable<TC>> tests, Action<TKey, TC, Action<TValue>> test)
        {
            foreach (var key in _possible.Keys)
            {
                foreach (var t in tests(key))
                {
                    var options = _possible[key];
                    test(key, t, value => options.Remove(value));
                }
            }
        }

        // If an option only has one possibility, remove that possibility
        // from every other option. Repeat until all options have only one
        // possibility.
        public bool ReduceToSingles()
        {
            var done = new HashSet<TKey>();
            for (var i = 0; i < _possible.Count - 1; i++)
            {
                var exists = _possible.WhereKey(k => !done.Contains(k)).WhereValue(options => options.Count == 1).First(out var option);
                if (!exists) return false;
                var (key, value) = option;
                done.Add(key);
                var remove = value.First();
                foreach (var k in _possible.Keys.Without(key))
                {
                    _possible[k].Remove(remove);
                }
            }
            return true;
        }

        public IEnumerable<TKey> Keys => _possible.Keys;

        public IEnumerable<TValue> Values => _values;

        public IEnumerable<KeyValuePair<TKey, IReadOnlySet<TValue>>> Options
        {
            get => _possible.Select(pair => new KeyValuePair<TKey, IReadOnlySet<TValue>>(pair.Key, pair.Value));
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Results => _possible.Select(pair => new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.First()));
    }
}