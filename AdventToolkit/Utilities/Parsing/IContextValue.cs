using AdventToolkit.Common;

namespace AdventToolkit.Utilities.Parsing;

public interface IContextValue<out T, in TContext>
{
    T GetValue(TContext context);
}

public static class ContextValueExtensions
{
    public static T GetValue<T>(this IContextValue<T, Unit> value)
    {
        return value.GetValue(default);
    }
}