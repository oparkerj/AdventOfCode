using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Collections.Space;
using AdventToolkit.Common;
using AdventToolkit.Extensions;

namespace AdventToolkit.Solvers;

public class GameOfLife : GameOfLife<Pos, bool>
{
    public GameOfLife() : base(true, false, () => new Grid<bool>(false)) { }

    public GameOfLife(bool includeCorners) : base(true, false, () => new Grid<bool>(includeCorners)) { }
        
    public GameOfLife(Func<AlignedSpace<Pos, bool>> cons) : base(true, false, cons) { }

    public static GameOfLife<Pos, T> OnGrid<T>(T alive, T dead, bool corners = false)
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

    public TState Default
    {
        get => _locations.Default;
        set
        {
            _locations.Default = value;
            _temp.Default = value;
        }
    }

    public GameOfLife(TState alive, TState dead)
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

    public AlignedSpace<TLoc, TState> Space => _locations;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<KeyValuePair<TLoc, TState>> GetEnumerator() => _locations.GetEnumerator();

    public TState this[TLoc loc]
    {
        get => _locations[loc];
        set => _locations[loc] = value;
    }

    public void CopySpace(AlignedSpace<TLoc, TState> space)
    {
        foreach (var (key, value) in space)
        {
            this[key] = value;
        }
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

    public void StepAnd(int count, Action<GameOfLife<TLoc, TState>> after)
    {
        count.Times(() =>
        {
            Step();
            after(this);
        });
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

    public IEnumerable<TPos> NeighborPositions() => Game.NeighborFunction(Pos);

    public IEnumerable<TState> Neighbors()
    {
        return Game.NeighborFunction(Pos).Select(pos => Game[pos]);
    }

    public int AliveNear() => CountNear(Game.Alive);
}

public class GameOfLife<T> : GameOfLife<T, bool>
{
    public GameOfLife() : base(true, false) { }
        
    public GameOfLife(Func<AlignedSpace<T, bool>> cons) : base(true, false, cons) { }

    public GameOfLife(bool alive, bool dead) : base(alive, dead) { }
        
    public GameOfLife(bool alive, bool dead, Func<AlignedSpace<T, bool>> cons) : base(alive, dead, cons) { }
}