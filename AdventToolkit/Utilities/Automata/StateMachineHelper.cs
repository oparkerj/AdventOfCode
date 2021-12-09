using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Automata;

// Keeps track of the last-used states to assist in state machine creation.
public class StateMachineHelper<T>
{
    public readonly StateMachine<T> Machine;
    public readonly HashSet<int> ToNext = new();
    public readonly HashSet<int> Lazy = new();
    private bool _finished = false;

    public StateMachineHelper() : this(new StateMachine<T>()) { }

    public StateMachineHelper(StateMachine<T> machine)
    {
        Machine = machine;
        if (Machine.Length < 1) Machine.NewState();
        ToNext.Add(Machine.InitialState);
    }

    // Connect two state machines.
    // This is done by copying the links from the second state machine into the first.
    //
    public static int Concat(StateMachine<T> a, IEnumerable<int> from, HashSet<int> lazy, StateMachine<T> b, HashSet<int> bLazy)
    {
        var offset = a.Length - 1;
        var connect = from.ToList();
        foreach (var (state, links) in b.Table)
        {
            if (state == 0)
            {
                foreach (var i in connect)
                {
                    a[i].AddRange(links.Offset(offset), lazy.Remove(i));
                }
            }
            else
            {
                a[state + offset].AddRange(links.Offset(offset));
            }
        }
        lazy.UnionWith(bLazy.Select(i => i + offset));
        a.RecalculateNext();
        return offset;
    }

    private void UpdateLazy(LinkType linkType = LinkType.Default)
    {
        if (linkType == LinkType.Lazy)
        {
            Lazy.UnionWith(ToNext);
        }
    }

    private bool UseLazy(int state)
    {
        return Lazy.Remove(state);
    }

    public static StateMachineHelper<T> BuildState(Action<StateMachineHelper<T>> action)
    {
        var h = new StateMachineHelper<T>();
        action(h);
        return h;
    }

    public static StateMachineHelper<T> BuildAndFinish(Action<StateMachineHelper<T>> action)
    {
        var helper = BuildState(action);
        helper.Finish();
        return helper;
    }

    public Action<StateMachineHelper<T>> Save(Action<StateMachineHelper<T>> action) => action;

    public StateMachine<T> Build() => Finish().Machine;

    // Finalize a state machine by setting the accepting states
    // and adding links for any lazy states.
    public StateMachineHelper<T> Finish()
    {
        if (_finished) return this;
        _finished = true;
        Machine.AcceptingStates.Clear();
        Move<T> move = null;
        if (ToNext.Intersect(Lazy).Any())
        {
            move = new Move<T>(Machine.NewState());
            Machine.AcceptingStates.Add(move.To);
        }
        foreach (var i in ToNext)
        {
            if (Lazy.Contains(i))
            {
                Machine[i].Add(move, true);
                // Machine.AcceptingStates.Add(move!.To);
            }
            else
            {
                Machine.AcceptingStates.Add(i);
            }
        }
        return this;
    }

    // Prepare to continue adding to a state machine.
    // This removes the special links that are added for lazy states.
    public StateMachineHelper<T> Continue()
    {
        if (!_finished) return this;
        _finished = false;
        Machine.AcceptingStates.Clear();
        foreach (var i in ToNext.Intersect(Lazy))
        {
            Machine[i].Possible.RemoveAll(link => link is Move<T>);
        }
        Machine.RecalculateNext();
        return this;
    }

    // Rewind the helper so that future links are added to the initial state.
    public StateMachineHelper<T> Rewind()
    {
        ToNext.Clear();
        ToNext.Add(Machine.InitialState);
        return this;
    }

    // Use the creation function and add the link to all open states.
    public StateMachineHelper<T> AddLink(Func<int, Link<T>> func)
    {
        var next = Machine.NewState();
        var link = func(next);
        foreach (var state in ToNext)
        {
            Machine[state].Add(link, UseLazy(state));
        }
        ToNext.Clear();
        ToNext.Add(next);
        return this;
    }

    // Assert that the current position is at the beginning of the input
    public StateMachineHelper<T> Beginning() => AddLink(next => new Beginning<T>(next));
        
    // Assert that the current position is at the end of the input
    public StateMachineHelper<T> Ending() => AddLink(next => new End<T>(next));

    // Assert that the next input is the specified value
    public StateMachineHelper<T> Then(T update)
    {
        return AddLink(next => new Single<T>(update, next));
    }

    // Insert a sequence of updates
    public StateMachineHelper<T> Then(IEnumerable<T> updates)
    {
        foreach (var update in updates)
        {
            Then(update);
        }
        return this;
    }
        
    // Assert that the next input matches the predicate
    public StateMachineHelper<T> Then(Func<T, bool> matcher)
    {
        return AddLink(next => new Class<T>(matcher, next));
    }
        
    public StateMachineHelper<T> Pick(IEnumerable<T> updates)
    {
        return AddLink(next => new OneOf<T>(updates.ToArray(), next));
    }

    public StateMachineHelper<T> ZeroOrMore(Action<StateMachineHelper<T>> action, LinkType linkType = LinkType.Default)
    {
        return ZeroOrMore(BuildState(action), linkType);
    }
        
    public StateMachineHelper<T> ZeroOrMore(StateMachineHelper<T> section, LinkType linkType = LinkType.Default)
    {
        var currentNext = new List<int>(ToNext);
        OneOrMore(section, linkType);
        ToNext.UnionWith(currentNext);
        UpdateLazy(linkType);
        return this;
    }

    public StateMachineHelper<T> OneOrMore(Action<StateMachineHelper<T>> action, LinkType linkType = LinkType.Default)
    {
        return OneOrMore(BuildState(action), linkType);
    }
        
    public StateMachineHelper<T> OneOrMore(StateMachineHelper<T> section, LinkType linkType = LinkType.Default)
    {
        if (section.Machine.Length < 2) return this;
        // Connect ends to beginnings to allow more than one
        var fromStart = section.Machine.Table.Where(pair => pair.Key == section.Machine.InitialState).ToList();
        foreach (var i in section.ToNext)
        {
            foreach (var (_, links) in fromStart)
            {
                section.Machine[i].AddRange(links, section.Lazy.Contains(i));
            }
            section.UseLazy(i);
        }
        var offset = Concat(Machine, ToNext, Lazy, section.Machine, section.Lazy);
        ToNext.Clear();
        ToNext.UnionWith(section.ToNext.Select(i => i + offset));
        UpdateLazy(linkType);
        return this;
    }

    public StateMachineHelper<T> ZeroOrOne(Action<StateMachineHelper<T>> action, LinkType linkType = LinkType.Default)
    {
        return ZeroOrOne(BuildState(action), linkType);
    }
        
    public StateMachineHelper<T> ZeroOrOne(StateMachineHelper<T> section, LinkType linkType = LinkType.Default)
    {
        if (section.Machine.Length < 2) return this;
        var offset = Concat(Machine, ToNext, Lazy, section.Machine, section.Lazy);
        ToNext.UnionWith(section.ToNext.Select(i => i + offset));
        UpdateLazy(linkType);
        return this;
    }

    public StateMachineHelper<T> Range(Action<StateMachineHelper<T>> action, Range range, LinkType linkType = LinkType.Default)
    {
        return Range(BuildState(action), range, linkType);
    }
        
    public StateMachineHelper<T> Range(StateMachineHelper<T> action, Range range, LinkType linkType = LinkType.Default)
    {
        if (range.Start.IsFromEnd) throw new ArgumentException("Index cannot be from end.");
        var start = range.Start.Value;
        var end = range.End.Value - start;
        for (var i = 0; i < start; i++)
        {
            Choice(action.EnumerateSingle());
        }
        if (range.End.IsFromEnd)
        {
            ZeroOrMore(action, linkType);
        }
        else
        {
            var next = new HashSet<int>();
            for (var i = 0; i < end; i++)
            {
                next.UnionWith(ToNext);
                Choice(action.EnumerateSingle());
            }
            ToNext.UnionWith(next);
            UpdateLazy(linkType);
        }
        return this;
    }

    public StateMachineHelper<T> Choice(IEnumerable<Action<StateMachineHelper<T>>> actions)
    {
        return Choice(actions.Select(BuildState));
    }
        
    public StateMachineHelper<T> Choice(IEnumerable<StateMachineHelper<T>> sections)
    {
        var offsets = sections.Select(helper =>
        {
            var offset = Concat(Machine, ToNext, Lazy, helper.Machine, helper.Lazy);
            return (helper, offset);
        }).ToList();
        if (offsets.Count == 0) return this;
        ToNext.Clear();
        foreach (var (helper, offset) in offsets)
        {
            ToNext.UnionWith(helper.ToNext.Select(i => i + offset));
        }
        return this;
    }

    public StateMachineHelper<T> Not(T update) => AddLink(next => new Not<T>(new Single<T>(update, next)));

    public StateMachineHelper<T> Except(IEnumerable<T> updates)
    {
        return AddLink(next => new NoneOf<T>(updates.ToArray(), next));
    }

    public StateMachineHelper<T> PositiveLookahead(Action<StateMachineHelper<T>> action)
    {
        return PositiveLookahead(BuildState(action));
    }

    public StateMachineHelper<T> PositiveLookahead(StateMachineHelper<T> section)
    {
        foreach (var links in section.Machine.Table.Values)
        {
            links.Consume(false);
        }
        return Choice(section.EnumerateSingle());
    }

    public StateMachineHelper<T> NegativeLookahead(Action<StateMachineHelper<T>> action)
    {
        return NegativeLookahead(BuildState(action));
    }

    public StateMachineHelper<T> NegativeLookahead(StateMachineHelper<T> section)
    {
        return AddLink(next => new Not<T>(new SubStateMachine<T>(section.Machine, next, false)));
    }

    public StateMachineHelper<T> Recurse(StateMachineHelper<T> section)
    {
        return Recurse(section.Machine);
    }
        
    public StateMachineHelper<T> Recurse(StateMachine<T> section)
    {
        return AddLink(next => new SubStateMachine<T>(section, next));
    }
}