namespace AdventToolkit.Utilities.Parsing
{
    public interface IContextValue<out T, in TContext>
    {
        T GetValue(TContext context);
    }

    public sealed class NoContext
    {
        private NoContext() { }
    }

    public static class ContextValueExtensions
    {
        public static T GetValue<T>(this IContextValue<T, NoContext> value)
        {
            return value.GetValue(null);
        }
    }
}