using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdventToolkit.Extensions;

public static class JsonExtensions
{
    public static bool HasValue<T>(this JObject obj, T value)
    {
        return obj.PropertyValues().WhereType<JValue>().Any(j => j.Value is T t && Equals(t, value));
    }
}