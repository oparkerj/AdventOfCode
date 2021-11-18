using System;
using System.Linq;

namespace AdventToolkit.Utilities.Automata
{
    public abstract class Link<T>
    {
        public int To;
        public bool Consume;

        protected Link(int to, bool consume)
        {
            To = to;
            Consume = consume;
        }

        public Link<T> ShallowCopy()
        {
            return (Link<T>) MemberwiseClone();
        }

        public virtual bool TryMatchOne(T update) => throw new NotSupportedException();

        public virtual bool TryMatch(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            var match = Match(input, index, out end, ref backtrack);
            if (!Consume) end = index;
            return match;
        }

        protected abstract bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack);
    }

    public class Move<T> : Link<T>
    {
        public Move(int to) : base(to, false) { }

        public override bool TryMatchOne(T update)
        {
            return true;
        }

        public override bool TryMatch(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index;
            return true;
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index;
            return true;
        }
    }

    public class Beginning<T> : Link<T>
    {
        public Beginning(int to) : base(to, false) { }
        
        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index;
            return index == 0;
        }
    }

    public class End<T> : Link<T>
    {
        public End(int to) : base(to, false) { }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index;
            return index == input.Length;
        }
    }

    public class Single<T> : Link<T>
    {
        public readonly T Value;

        public Single(T value, int to, bool consume = true) : base(to, consume)
        {
            Value = value;
        }

        public override bool TryMatchOne(T update)
        {
            return Equals(Value, update);
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index + 1;
            return index < input.Length && Equals(input[index], Value);
        }
    }

    public class Class<T> : Link<T>
    {
        public readonly Func<T, bool> Matcher;

        public Class(Func<T, bool> matcher, int to, bool consume = true) : base(to, consume)
        {
            Matcher = matcher;
        }

        public override bool TryMatchOne(T update)
        {
            return Matcher(update);
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index + 1;
            return index < input.Length && Matcher(input[index]);
        }
    }

    public class Not<T> : Link<T>
    {
        public readonly Link<T> Link;

        public Not(Link<T> link) : base(link.To, link.Consume)
        {
            Link = link;
        }

        public override bool TryMatchOne(T update)
        {
            return !Link.TryMatchOne(update);
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index;
            return index < input.Length && !Link.TryMatch(input, index, out end, ref backtrack);
        }
    }

    public class OneOf<T> : Link<T>
    {
        public readonly T[] Values;

        public OneOf(T[] values, int to, bool consume = true) : base(to, consume)
        {
            Values = values;
        }

        public override bool TryMatchOne(T update)
        {
            return Values.Any(t => Equals(t, update));
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index + 1;
            return index < input.Length && Values.Any(t => Equals(input[index], t));
        }
    }

    public class NoneOf<T> : Link<T>
    {
        public readonly T[] Values;

        public NoneOf(T[] values, int to, bool consume = true) : base(to, consume)
        {
            Values = values;
        }

        public override bool TryMatchOne(T update)
        {
            return !Values.Any(t => Equals(t, update));
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            end = index + 1;
            return index < input.Length && !Values.Any(t => Equals(input[index], t));
        }
    }

    public class SubStateMachine<T> : Link<T>
    {
        public readonly StateMachine<T> Machine;

        public SubStateMachine(StateMachine<T> machine, int to, bool consume = true) : base(to, consume)
        {
            Machine = machine;
        }

        protected override bool Match(T[] input, int index, out int end, ref Backtrack<T> backtrack)
        {
            var match = Machine.Match(input, out var path, ref backtrack, index);
            end = path[^1].index;
            return match;
        }
    }
}