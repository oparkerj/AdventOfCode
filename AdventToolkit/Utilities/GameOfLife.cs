using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class GameOfLife<TLoc, TState> : IEnumerable<KeyValuePair<TLoc, TState>>
    {
        private DefaultDict<TLoc, TState> _locations = new();
        private DefaultDict<TLoc, TState> _temp = new();
        private Queue<TLoc> _queue = new();
        private HashSet<TLoc> _checked = new();

        public TState Alive;
        public TState Dead;
        public Func<int, TState, TState> UpdateFunction;
        public Func<TLoc, IEnumerable<TLoc>> NeighborFunction;
        public bool Expanding;
        public bool KeepDead = true;

        public GameOfLife(TState dead, TState alive)
        {
            Alive = alive;
            Dead = dead;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TLoc, TState>> GetEnumerator()
        {
            return _locations.GetEnumerator();
        }

        public TState this[TLoc loc]
        {
            get => _locations[loc];
            set => _locations[loc] = value;
        }

        public GameOfLife<TLoc, TState> WithUpdateFunction(Func<int, TState, TState> func)
        {
            UpdateFunction = func;
            return this;
        }

        public GameOfLife<TLoc, TState> WithLivingDeadRules(Func<int, bool> alive, Func<int, bool> dead)
        {
            return WithUpdateFunction((i, state) =>
            {
                if (state.Equals(Alive) && alive(i)) return Dead;
                if (state.Equals(Dead) && dead(i)) return Alive;
                return state;
            });
        }

        public GameOfLife<TLoc, TState> WithNeighborFunction(Func<TLoc, IEnumerable<TLoc>> func)
        {
            NeighborFunction = func;
            return this;
        }

        public GameOfLife<TLoc, TState> WithExpansion(bool expand = true)
        {
            Expanding = expand;
            return this;
        }

        public GameOfLife<TLoc, TState> WithKeepDead(bool keep)
        {
            KeepDead = keep;
            return this;
        }

        public bool Has(TLoc loc)
        {
            return _locations.ContainsKey(loc);
        }

        public void Step(int times)
        {
            times.Times(() => Step());
        }

        public int Step()
        {
            var c = 0;
            foreach (var key in _locations.Keys)
            {
                _queue.Enqueue(key);
            }
            while (_queue.Count > 0)
            {
                var loc = _queue.Dequeue();
                if (Expanding) _checked.Add(loc);
                var state = _locations[loc];
                if (!state.Equals(Alive) && !state.Equals(Dead))
                {
                    _temp[loc] = state;
                    continue;
                }
                var original = Has(loc);
                var neighbors = NeighborFunction(loc);
                var count = 0;
                foreach (var near in neighbors)
                {
                    if (_locations.TryGetValue(near, out var nearState))
                    {
                        if (nearState.Equals(Alive)) count++;
                    }
                    else if (Expanding && original && !_checked.Contains(near)) _queue.Enqueue(near);
                }
                var after = UpdateFunction(count, state);
                if (!after.Equals(Dead) || KeepDead) _temp[loc] = after;
                if (!after.Equals(state)) c++;
            }
            Data.Swap(ref _locations, ref _temp);
            _temp.Clear();
            _checked.Clear();
            return c;
        }

        public void StepUntil(Func<int, bool> func)
        {
            var last = 0;
            do
            {
                last = Step();
            } while (!func(last));
        }
    }

    public class GameOfLife<T> : GameOfLife<T, bool>
    {
        public GameOfLife() : base(false, true) { }

        public GameOfLife(bool dead, bool alive) : base(dead, alive) { }
    }

    public static class GameOfLifeExtensions
    {
        public static int CountActive<T>(this GameOfLife<T, bool> game)
        {
            return game.Count(pair => pair.Value);
        }
        
        public static Grid<T> ToGrid<T>(this GameOfLife<Pos, T> game)
        {
            var grid = new Grid<T>();
            foreach (var (pos, value) in game)
            {
                grid[pos] = value;
            }
            return grid;
        }
    }
}