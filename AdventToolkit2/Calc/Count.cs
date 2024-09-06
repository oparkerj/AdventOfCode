using System.Diagnostics;
using System.Numerics;
using AdventToolkit2.Alg.Step.Interface;
using AdventToolkit2.Collections;
using AdventToolkit2.Debugging;
using AdventToolkit2.Extensions;

namespace AdventToolkit2.Calc;

public static class Count<TNum>
    where TNum : INumber<TNum>
{
    /// <summary>
    /// Get the number of cycles needed for the step to cycle back around to itself.
    /// This assumes that the step will eventually return to an identical state at
    /// some point.
    /// </summary>
    /// <param name="step"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public static TNum FindCycle<T, TState>(T step)
        where T : IStep<T, TState>
    {
        var first = step.State;
        var count = TNum.Zero;

        do
        {
            step = step.Next();
            ++count;
        } while (Equality.Ne(first, step.State));

        return count;
    }

    /// <inheritdoc cref="FindCycle{T,TState}(T)"/>
    public static TNum FindCycle<T, TState>(IStep<T, TState> step)
        where T : IStep<T, TState>
    {
        // Would prefer to just use T as the argument type, but that's not possible
        // without forcing the caller to specify all the generics.
        // Technically it is possible for this cast to fail, but under normal cases,
        // T should be the underlying type.
        Debug.Assert(step is T);
        return FindCycle<T, TState>((T) step);
    }
    
    /// <summary>
    /// Determine when the step begins to repeat.
    /// This calculates both the cycle offset and cycle length.
    /// This assumes that the step will eventually return to a previous state.
    /// </summary>
    /// <param name="step"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <returns>A tuple containing the number of steps before the state begins to cycle,
    /// and the cycle length.</returns>
    public static (TNum Offset, TNum Cycle) FindCycleOffset<T, TState>(T step)
        where T : IStep<T, TState>
        where TState : notnull
    {
        var state = step.State;
        
        var states = new Dictionary<TState, TNum>();
        var counter = TNum.Zero;
        TNum offset;

        do
        {
            states[state] = counter++;
            step = step.Next();
            state = step.State;
        } while (!states.TryGetValue(state, out offset!));

        return (offset, counter - offset);
    }

    /// <inheritdoc cref="FindCycleOffset{T,TState}(T)"/>
    public static (TNum Offset, TNum Cycle) FindCycleOffset<T, TState>(IStep<T, TState> step)
        where T : IStep<T, TState>
        where TState : notnull
    {
        Debug.Assert(step is T);
        return FindCycleOffset<T, TState>((T) step);
    }
    
    /// <summary>
    /// Find the total cycle time of multiple steps at once.
    /// This is the number of steps that must be applied to the group of steppers for all
    /// of them to simultaneously return to the same starting step.
    /// This is done by finding the individual cycle times and taking the LCM.
    /// </summary>
    /// <param name="steps"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TNum"></typeparam>
    /// <returns></returns>
    public static TNum FindTotalCycle<T, TState>(IEnumerable<T> steps)
        where T : IStep<T, TState>
    {
        using var e = steps.GetEnumerator();
        if (!e.MoveNext()) Err.EndOfSequence();

        var result = FindCycle<T, TState>(e.Current);
        while (e.MoveNext())
        {
            result = result.Lcm(FindCycle<T, TState>(e.Current));
        }
        return result;
    }

    /// <inheritdoc cref="FindTotalCycle{T,TState}(System.Collections.Generic.IEnumerable{T})"/>
    public static TNum FindTotalCycle<T, TState>(IEnumerable<IStep<T, TState>> steps)
        where T : IStep<T, TState>
    {
        return FindTotalCycle<T, TState>(steps.Cast<T>());
    }

    /// <summary>
    /// Find the cycle period for a collection of sim steps.
    /// This will find the number of cycles for all steps to simultaneously return
    /// to their initial state.
    /// </summary>
    /// <param name="steps"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TUpdate"></typeparam>
    /// <returns></returns>
    public static TNum FindSimCycle<T, TState, TUpdate>(IEnumerable<T> steps)
        where T : ISimStep<T, TState, TUpdate>
    {
        var list = steps.ToList();
        var count = list.Count;
        using Arr<TState> states = new(count);
        using Arr<TUpdate> updates = new(count);

        // Get the initial states
        for (var i = 0; i < count; i++)
        {
            states[i] = list[i].State;
        }

        var counter = TNum.One;
        while (true)
        {
            // Get the update values
            for (var i = 0; i < count; i++)
            {
                updates[i] = list[i].Affect(list.Exclude(i, 1));
            }
            
            // Apply the updates
            var all = true;
            for (var i = 0; i < count; i++)
            {
                var next = list[i].Apply(updates[i]);
                list[i] = next;
                if (all && Equality.Ne(next.State, states[i]))
                {
                    all = false;
                }
            }
            
            // If all steps returned to the original state, we're done
            if (all) break;
            counter++;
        }

        return counter;
    }
}