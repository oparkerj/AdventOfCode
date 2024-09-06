namespace AdventToolkit2.Alg.Step.Interface;

/// <summary>
/// A sim step is one that is affected by other steps.
/// For example when every element affects every other element on each step.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TUpdate"></typeparam>
public interface ISimStep<T, out TState, TUpdate> : IStep<T, TState>
    where T : ISimStep<T, TState, TUpdate>
{
    /// <summary>
    /// Aggregate the influence that other items have on the current item.
    /// This function returns a value that can be passed to <see cref="Apply"/>
    /// to get the next step after the influence is applied.
    /// </summary>
    /// <param name="others"></param>
    /// <returns></returns>
    TUpdate Affect(IEnumerable<T> others);

    /// <summary>
    /// Apply the update value to get the next step.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    T Apply(TUpdate update);
}