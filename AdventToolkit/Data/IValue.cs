namespace AdventToolkit.Data
{
    public interface IContextValue<out T, in TContext>
    {
        T GetValue(TContext context);
    }
}