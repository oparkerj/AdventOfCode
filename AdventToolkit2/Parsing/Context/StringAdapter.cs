using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// String adapter.
///
/// This allows types to be converted to string if they have overloaded
/// the ToString method.
///
/// TODO Allow higher priority adapters.
/// This adapter is automatically added to the default context, so it is
/// checked before any custom adapters. At the moment, this adapter will only
/// convert to string if the ToString method has been overloaded. This makes it
/// possible to still write a custom string conversion for types that do not
/// overload ToString.
/// </summary>
public class StringAdapter : IAdapterLookupByTarget<string>
{
    public bool TryLookup(Type from, IParseContext context, out IParser parser)
    {
        if (from.GetMethod(nameof(ToString), [])?.DeclaringType is { } declType && declType != typeof(object))
        {
            parser = typeof(ToString<>).NewParserGeneric([from]);
            return true;
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Call ToString on the input, or output "null" if the input is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToString<T> : IParser<T, string>
    {
        public string Parse(T input) => input?.ToString() ?? "null";
    }
}