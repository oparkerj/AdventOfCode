using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Builtin;

public static class IdentityAdapter
{
    public static IParser Create(Type type)
    {
        var adapter = typeof(IdentityAdapter<>).MakeGenericType(type);
        return (IParser) adapter.GetProperty(nameof(IdentityAdapter<int>.Instance))!.GetValue(null)!;
    }
}

public class IdentityAdapter<T> : IParser<T, T>
{
    private static IdentityAdapter<T>? _instance;

    public static IdentityAdapter<T> Instance => _instance ??= new IdentityAdapter<T>();
    
    public T Parse(T input) => input;
}