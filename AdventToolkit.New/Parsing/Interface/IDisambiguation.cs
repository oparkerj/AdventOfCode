using System.Diagnostics;

namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// Specifies a disambiguation type.
/// </summary>
public interface IDisambiguation
{
    /// <summary>
    /// Test if this disambiguation applies to the given type.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    static abstract bool Match(Type self, Type type);

    /// <summary>
    /// Calls <see cref="Match"/> on the given type.
    /// </summary>
    /// <param name="self">Disambiguation type.</param>
    /// <param name="type">Current disambiguation test.</param>
    /// <returns></returns>
    static bool Matches(Type self, Type type)
    {
        Debug.Assert(self.IsAssignableTo(typeof(IDisambiguation)));
        return (bool) self.GetMethod(nameof(Match), [typeof(Type), typeof(Type)])!.Invoke(null, [self, type])!;
    }

    /// <summary>
    /// Get the next disambiguation state.
    /// 
    /// For example: A type with generics may return one of the generic types as the next state.
    /// </summary>
    /// <param name="self"></param>
    /// <returns>Next state, or null if there is no further state.</returns>
    static abstract Type? Apply(Type self);

    /// <summary>
    /// Calls <see cref="Apply"/> on the given type.
    /// </summary>
    /// <param name="self">Disambiguation type.</param>
    /// <returns></returns>
    static Type? ApplyTo(Type self)
    {
        Debug.Assert(self.IsAssignableTo(typeof(IDisambiguation)));
        return (Type?) self.GetMethod(nameof(Apply), [typeof(Type)])!.Invoke(null, [self]);
    }
}

/// <summary>
/// A disambiguation which automatically applies when it is reached,
/// without testing for a match.
/// </summary>
public interface IAutoDisambiguation : IDisambiguation;