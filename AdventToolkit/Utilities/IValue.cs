namespace AdventToolkit.Utilities
{
    public interface IContextValue<out T, in TContext>
    {
        T GetValue(TContext context);

        T GetValue() => GetValue(default);
    }
}