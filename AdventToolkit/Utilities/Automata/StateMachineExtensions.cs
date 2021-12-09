using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventToolkit.Utilities.Automata;

public static class StateMachineExtensions
{
    public static bool Match<T>(this StateMachine<T> machine, T[] updates, out List<(int start, int index)> path, int start = 0)
    {
        Backtrack<T> b = default;
        return machine.Match(updates, out path, ref b, start);
    }
        
    public static bool Match<T>(this StateMachine<T> machine, IEnumerable<T> updates, out List<(int start, int index)> path, int start = 0)
    {
        Backtrack<T> b = default;
        return machine.Match(updates.ToArray(), out path, ref b, start);
    }

    public static StateMachineHelper<T> Choice<T>(this StateMachineHelper<T> helper, params Action<StateMachineHelper<T>>[] actions)
    {
        return helper.Choice(actions);
    }
        
    public static StateMachineHelper<T> Choice<T>(this StateMachineHelper<T> helper, params StateMachineHelper<T>[] sections)
    {
        return helper.Choice(sections);
    }
}