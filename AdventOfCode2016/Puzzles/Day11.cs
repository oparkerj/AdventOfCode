using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;
using MoreLinq;
using RegExtract;

namespace AdventOfCode2016.Puzzles;

public class Day11 : Puzzle
{
    public const int Chip = 0;
    public const int Generator = 1;
    public const int Floors = 4;

    private State ReadFloors()
    {
        var state = new State(0);

        var floor = 0;
        foreach (var s in Input[..^1])
        {
            var content = s.After("contains ");
            
            foreach (var part in content.Split(','))
            {
                var (kind, type) = part.Extract<(string, string)>(@"(?:and )?a (\w+)(?:\-compatible)? (\w+)");

                var component = new Component(type == "generator" ? Generator : Chip, kind);
                state.Put(component, floor);
            }

            floor++;
        }

        if (Part == 2)
        {
            // I have no idea if part two works, it takes way too long to run.
            // But it is known that placing these objects in this manner adds 24 moves to part one.
            state.Put(new Component(Generator, "elerium"), 0);
            state.Put(new Component(Chip, "elerium"), 0);
            state.Put(new Component(Generator, "dilithium"), 0);
            state.Put(new Component(Chip, "dilithium"), 0);
        }

        return state;
    }

    public override void PartOne()
    {
        var path = new Dijkstra<State>
        {
            Neighbors = state => state.GetNeighbors(),
            Distance = _ => 1
        };

        var start = ReadFloors();
        var end = new State(start, Floors - 1);
        end.MoveAll(Floors - 1);

        var steps = path.ComputeFind(start, end, state => state.IsValid());
        Clip(steps);
    }

    public class State
    {
        private Dictionary<Component, int> _components;
        private int _floor;

        private int? _hash;

        public State(int floor)
        {
            _components = new Dictionary<Component, int>();
            _floor = floor;
        }

        public State(State other, int floor)
        {
            _components = new Dictionary<Component, int>(other._components);
            _floor = floor;
        }

        private State Copy() => new(this, _floor);

        public State Put(Component component, int floor)
        {
            _components[component] = floor;
            return this;
        }

        public State On(int floor)
        {
            _floor = floor;
            return this;
        }

        public State MoveAll(int floor)
        {
            foreach (var component in _components.Keys.ToList())
            {
                _components[component] = floor;
            }
            return this;
        }

        private IEnumerable<Component> OnFloor(int floor)
        {
            return _components.Where(pair => pair.Value == floor).Keys();
        }

        public bool IsValid()
        {
            for (var i = 0; i < Floors; i++)
            {
                var floor = OnFloor(i).ToList();
                if (floor.All(component => component.Type != Generator)) continue;
                if (floor.Where(component => component.Type == Chip).Select(chip => chip.Other).Any(other => !floor.Contains(other)))
                {
                    return false;
                }
            }
            return true;
        }

        public IEnumerable<State> GetNeighbors()
        {
            var down = _floor > 0;
            var up = _floor < Floors - 1;

            var onFloor = OnFloor(_floor).ToList();

            foreach (var component in onFloor)
            {
                if (down) yield return Copy().On(_floor - 1).Put(component, _floor - 1);
                if (up) yield return Copy().On(_floor + 1).Put(component, _floor + 1);
            }

            if (onFloor.Count < 2) yield break;
            foreach (var components in onFloor.Subsets(2))
            {
                if (down)
                {
                    var copy = Copy().On(_floor - 1);
                    foreach (var component in components)
                    {
                        copy.Put(component, _floor - 1);
                    }
                    yield return copy;
                }
                if (up)
                {
                    var copy = Copy().On(_floor + 1);
                    foreach (var component in components)
                    {
                        copy.Put(component, _floor + 1);
                    }
                    yield return copy;
                }
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is State s && s._floor == _floor && s._components.ContentEquals(_components);
        }

        public override int GetHashCode()
        {
            if (_hash != null) return _hash.Value;
            var hash = 0;
            foreach (var (key, value) in _components.OrderBy(pair => pair.Key.GetHashCode()))
            {
                hash = HashCode.Combine(hash, key.GetHashCode(), value);
            }
            _hash = hash;
            return hash;
        }
    }

    public record Component(int Type, string Kind)
    {
        public Component Other => this with {Type = 1 - Type};
    }
}