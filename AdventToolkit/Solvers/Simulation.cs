using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Utilities.Arithmetic;
using MoreLinq;

namespace AdventToolkit.Solvers;

// Like game of life but every object affects
// every other object in each step.
public class Simulation<T>
{
    public readonly List<T> Objects = new();

    public Func<T, IEnumerable<T>, T> Update;
    public Action<T> Apply;

    public Simulation() { }

    public Simulation(IEnumerable<T> items) => Objects.AddRange(items);

    // Create an update function that aggregates values and then applies the final value
    public static Func<T, IEnumerable<T>, T> Aggregate<TA>(Func<T, T, TA> collect, Action<T, TA> write)
        where TA : IAdd<TA>
    {
        return (item, others) =>
        {
            var aggregate = others.Select(i => collect(item, i)).Aggregate((a, b) => a.Add(b));
            write(item, aggregate);
            return item;
        };
    }

    public Simulation<T> WithUpdate(Func<T, IEnumerable<T>, T> func)
    {
        Update = func;
        return this;
    }

    public Simulation<T> WithApply(Action<T> action)
    {
        Apply = action;
        return this;
    }

    public void Step()
    {
        var updated = Objects.Select((item, i) => Update(item, Objects.Exclude(i, 1))).ToList();
        updated.ForEach(Apply);
        Objects.Clear();
        Objects.AddRange(updated);
    }
}