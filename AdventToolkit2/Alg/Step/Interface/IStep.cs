using System.Numerics;

namespace AdventToolkit2.Alg.Step.Interface;

/// <summary>
/// Represents an abstract state that can advance to another state.
///
/// Interface-wise this is very similar to <see cref="IEnumerator{T}"/>,
/// but this interface also embeds the underlying type.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TState"></typeparam>
public interface IStep<out T, out TState>
    where T : IStep<T, TState>
{
    /// <summary>
    /// Current state.
    /// </summary>
    TState State { get; }
    
    /// <summary>
    /// Next state.
    /// </summary>
    /// <returns></returns>
    T Next();
}

/// <summary>
/// Similar to <see cref="IStep{T,TState}"/> but also embeds the numeric type
/// used to count the number of cycles.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TNum"></typeparam>
public interface IStep<out T, out TState, TNum> : IStep<T, TState>
    where T : IStep<T, TState, TNum>
    where TNum : INumber<TNum>;