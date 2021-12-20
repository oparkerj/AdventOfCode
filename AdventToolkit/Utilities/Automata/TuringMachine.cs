using System;
using System.Collections.Generic;
using AdventToolkit.Collections.Space;

namespace AdventToolkit.Utilities.Automata;

public class TuringMachine<T> : StateMachine<T>
{
    public readonly Dictionary<(int State, T Value), (T Write, bool MoveRight)> Update = new();

    public readonly Line<T> Tape = new();

    public int Reader = 0;
        
    public void RunSteps(int steps)
    {
        var tape = Tape;
        var state = InitialState;
        for (var i = 0; i < steps; i++)
        {
            var update = tape[Reader];
            if (!Update.TryGetValue((state, update), out var action)) throw new Exception("No action for current state.");
            tape[Reader] = action.Write;
            if (action.MoveRight) Reader++;
            else Reader--;
            if (!Step(state, update, out state)) throw new Exception("No target for current state.");
        }
    }
}