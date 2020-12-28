using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class StateMachine<TUpdate>
    {
        public readonly HashSet<int> States = new();
        public readonly DefaultDict<(int id, TUpdate symbol), HashSet<int>> Table = new();
        public readonly HashSet<int> AcceptingStates = new();

        public HashSet<int> this[(int id, TUpdate symbol) p] => Table[p] ??= new HashSet<int>(1);

        public bool IsNfa()
        {
            return Table.Values.Any(set => set.Count > 1);
        }

        public int NewState()
        {
            var next = States.Count;
            States.Add(next);
            return next;
        }

        public void Concat(StateMachine<TUpdate> other, int start = 0)
        {
            var offset = States.Count - 1;
            var count = other.States.Count;
            for (var i = 0; i < count; i++)
            {
                NewState();
            }
            foreach (var pair in other.Table)
            {
                if (pair.Key.id == start)
                {
                    foreach (var id in AcceptingStates)
                    {
                        Table[(id, pair.Key.symbol)] = pair.Value.Select(i => i + offset).ToHashSet();
                    }
                }
                else
                {
                    Table[(pair.Key.id + offset, pair.Key.symbol)] = pair.Value.Select(i => i + offset).ToHashSet();
                }
            }
            AcceptingStates.Clear();
            foreach (var id in other.AcceptingStates)
            {
                AcceptingStates.Add(id + offset);
            }
        }

        public bool Test(IEnumerable<TUpdate> updates)
        {
            var current = 0;
            foreach (var update in updates)
            {
                if (!Table.TryGetValue((current, update), out var states)) return false;
                if (states.Count > 1) throw new Exception("Automaton is non-deterministic.");
                current = states.First();
            }
            return AcceptingStates.Contains(current);
        }

        public int Match(IEnumerable<TUpdate> updates)
        {
            var current = 0;
            var index = 0;
            foreach (var update in updates)
            {
                if (!Table.TryGetValue((current, update), out var states)) return -1;
                if (states.Count > 1) throw new Exception("Automaton is non-deterministic.");
                current = states.First();
                if (AcceptingStates.Contains(current)) return index + 1;
                index++;
            }
            if (!AcceptingStates.Contains(current)) return -1;
            return index;
        }

        public (int count, int end) Count(IEnumerable<TUpdate> updates)
        {
            var current = 0;
            var index = 0;
            var count = 0;
            var end = 0;
            foreach (var update in updates)
            {
                if (!Table.TryGetValue((current, update), out var states)) break;
                if (states.Count > 1) throw new Exception("Automaton is non-deterministic.");
                current = states.First();
                if (AcceptingStates.Contains(current))
                {
                    count++;
                    end += index + 1;
                    index = 0;
                    current = 0;
                    continue;
                }
                index++;
            }
            return (count, end);
        }

        public StateMachine<TUpdate> NfaToDfa(IEnumerable<TUpdate> symbols)
        {
            var inputs = symbols.ToArray();
            var dfa = new StateMachine<TUpdate>();
            var sets = new Dictionary<int, int[]>();
            var queue = new Queue<int>();
            var first = dfa.NewState();
            sets[first] = new[] {0};
            queue.Enqueue(first);
            while (queue.Count > 0)
            {
                var s = queue.Dequeue();
                var state = sets[s];
                foreach (var input in inputs)
                {
                    var newState = state.Select(i => Table[(i, input)])
                        .Where(set => set != null)
                        .Flatten()
                        .OrderBy(i => i)
                        .ToArray();
                    var pairs = sets.Where(pair => pair.Value.SequenceEqual(newState)).ToList();
                    if (pairs.Count == 0)
                    {
                        var id = dfa.NewState();
                        sets[id] = newState;
                        dfa.Table[(s, input)] = new HashSet<int>(1) {id};
                        queue.Enqueue(id);
                    }
                    else
                    {
                        dfa.Table[(s, input)] = new HashSet<int>(1) {pairs[0].Key};
                    }
                }
            }
            var accepting = sets.Where(pair => pair.Value.Any(i => AcceptingStates.Contains(i)))
                .Select(pair => pair.Key);
            dfa.AcceptingStates.UnionWith(accepting);
            return dfa;
        }
    }
}