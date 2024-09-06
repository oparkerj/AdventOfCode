using System.Numerics;
using AdventToolkit2.Alg.Step.Interface;
using AdventToolkit2.Calc;

namespace AdventToolkit2.Alg.Step;

public static class StepExtensions
{
    /// <inheritdoc cref="Count{TNum}.FindCycle{T,TState}(T)"/>
    public static TNum FindCycle<T, TState, TNum>(IStep<T, TState, TNum> step)
        where T : IStep<T, TState, TNum>
        where TNum : INumber<TNum> =>
        Count<TNum>.FindCycle(step);

    /// <inheritdoc cref="Count{TNum}.FindCycleOffset{T,TState}(T)"/>
    public static (TNum Offset, TNum Cycle) FindCycleOffset<T, TState, TNum>(IStep<T, TState, TNum> step)
        where T : IStep<T, TState, TNum>
        where TState : notnull
        where TNum : INumber<TNum> =>
        Count<TNum>.FindCycleOffset(step);

    /// <inheritdoc cref="Count{TNum}.FindTotalCycle{T,TState}(System.Collections.Generic.IEnumerable{T})"/>
    public static TNum FindTotalCycle<T, TState, TNum>(IEnumerable<IStep<T, TState, TNum>> steps)
        where T : IStep<T, TState, TNum>
        where TNum : INumber<TNum> =>
        Count<TNum>.FindTotalCycle(steps);
}