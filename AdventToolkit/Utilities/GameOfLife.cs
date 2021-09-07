using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities
{
    public class GameOfLife
    {
        public static GameOfLife<Pos, T> OnGrid<T>(T dead, T alive, bool corners = false)
        {
            return new GameOfLife<Pos, T>(dead, alive, () => new Grid<T>(corners));
        }
    }
    
    public class GameOfLife<TLoc, TState> : IEnumerable<KeyValuePair<TLoc, TState>>
    {
        internal AlignedSpace<TLoc, TState> _locations;
        private AlignedSpace<TLoc, TState> _temp;
        private Queue<TLoc> _queue = new();
        private HashSet<TLoc> _checked = new();
        private GameOfLifeCell<TLoc, TState> _cell;

        public TState Alive;
        public TState Dead;
        // public Func<TLoc, int, TState, TState> UpdateFunction;
        public Func<GameOfLifeCell<TLoc, TState>, TState> UpdateFunction;
        public Func<TLoc, IEnumerable<TLoc>> NeighborFunction;
        public bool Expanding;
        public bool KeepDead = true;

        public GameOfLife(TState dead, TState alive)
        {
            Alive = alive;
            Dead = dead;
            _locations = new FreeSpace<TLoc, TState>();
            _temp = new FreeSpace<TLoc, TState>();
            _cell = new GameOfLifeCell<TLoc, TState>(this);
        }

        public GameOfLife(TState alive, TState dead, Func<AlignedSpace<TLoc, TState>> cons)
        {
            Alive = alive;
            Dead = dead;
            _locations = cons();
            _temp = cons();
            _cell = new GameOfLifeCell<TLoc, TState>(this);
            NeighborFunction = _locations.GetNeighbors;
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

        public GameOfLife<TLoc, TState> WithUpdate(Func<GameOfLifeCell<TLoc, TState>, TState> func)
        {
            UpdateFunction = func;
            return this;
        }

        public GameOfLife<TLoc, TState> WithUpdateUsingAlive(Func<TLoc, int, TState, TState> func)
        {
            return WithUpdate(update => func(update.Pos, update.AliveNear(), update.State));
        }

        public GameOfLife<TLoc, TState> WithLivingDeadRules(Func<int, bool> alive, Func<int, bool> dead)
        {
            return WithUpdateUsingAlive((_, i, state) =>
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
            return _locations.Has(loc);
        }

        public void Step(int count)
        {
            count.Times(() => Step());
        }

        // Step the game once and return the number of cells that changed states
        public int Step()
        {
            var c = 0;
            foreach (var key in _locations.Positions)
            {
                _queue.Enqueue(key);
            }
            while (_queue.Count > 0)
            {
                var loc = _queue.Dequeue();
                if (Expanding) _checked.Add(loc);
                var state = _locations[loc];
                // if (!state.Equals(Alive) && !state.Equals(Dead))
                // {
                //     _temp[loc] = state;
                //     continue;
                // }
                var original = Has(loc);
                // var neighbors = NeighborFunction(loc).ToList();
                // var count = 0;
                if (Expanding && original)
                {
                    foreach (var near in NeighborFunction(loc))
                    {
                        if (!_checked.Contains(near)) _queue.Enqueue(near);
                    }
                }
                // foreach (var near in neighbors)
                // {
                //     if (_locations.Lookup(near, out var nearState))
                //     {
                //         if (nearState.Equals(Alive)) count++;
                //     }
                //     else if (Expanding && original && !_checked.Contains(near)) _queue.Enqueue(near);
                // }
                _cell.Pos = loc;
                _cell.State = state;
                var after = UpdateFunction(_cell);
                if (!after.Equals(Dead) || KeepDead) _temp[loc] = after;
                if (!after.Equals(state)) c++;
            }
            Data.Swap(ref _locations, ref _temp);
            _temp.Clear();
            _checked.Clear();
            return c;
        }

        // Step until the number of cells changed passes the predicate
        public void StepUntil(Func<int, bool> func)
        {
            var last = 0;
            do
            {
                last = Step();
            } while (!func(last));
        }
    }

    public class GameOfLifeCell<TPos, TState>
    {
        public readonly GameOfLife<TPos, TState> Game;
        public TPos Pos;
        public TState State;

        public GameOfLifeCell(GameOfLife<TPos, TState> game)
        {
            Game = game;
        }

        public int CountNear(TState state)
        {
            return Game.NeighborFunction(Pos)
                .GetFrom(Game._locations)
                .Count(s => Equals(s, state));
        }

        public int AliveNear() => CountNear(Game.Alive);
    }

    public class GameOfLife<T> : GameOfLife<T, bool>
    {
        public GameOfLife() : base(false, true) { }
        
        public GameOfLife(Func<AlignedSpace<T, bool>> cons) : base(false, true, cons) { }

        public GameOfLife(bool dead, bool alive) : base(dead, alive) { }
        
        public GameOfLife(bool dead, bool alive, Func<AlignedSpace<T, bool>> cons) : base(dead, alive, cons) { }
    }
}