using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Builtin;

/// <summary>
/// Helper methods for the identity adapter.
/// </summary>
public static class IdentityAdapter
{
    /// <summary>
    /// Create an identity adapter.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IParser Create(Type type)
    {
        var adapter = typeof(IdentityAdapter<>).MakeGenericType(type);
        return (IParser) adapter.GetProperty(nameof(IdentityAdapter<int>.Instance))!.GetValue(null)!;
    }
}

/// <summary>
/// Parser which returns its input.
/// </summary>
/// <typeparam name="T"></typeparam>
public class IdentityAdapter<T> : IParser<T, T>
{
    private static IdentityAdapter<T>? _instance;

    public static IdentityAdapter<T> Instance => _instance ??= new IdentityAdapter<T>();
    
    public T Parse(T input) => input;
}