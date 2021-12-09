using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019.IntCode;

public class OutputCondition
{
    private readonly List<Condition> Cases = new();

    public OutputCondition Case(Func<long, bool> use, Action<long> action)
    {
        Cases.Add(new Condition(use, action));
        return this;
    }

    public OutputCondition CaseInt(Func<int, bool> use, Action<int> action)
    {
        return Case(data => use((int) data), data => action((int) data));
    }

    public OutputCondition Else(Action<long> action)
    {
        return Case(_ => true, action);
    }

    public OutputCondition ElseInt(Action<int> action)
    {
        return CaseInt(_ => true, action);
    }

    public void Line(long data)
    {
        Cases.FirstOrDefault(condition => condition.Use(data))?.Action(data);
    }

    private record Condition(Func<long, bool> Use, Action<long> Action);
}