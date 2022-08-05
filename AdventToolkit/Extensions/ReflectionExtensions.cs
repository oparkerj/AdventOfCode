using System;

namespace AdventToolkit.Extensions;

public static class ReflectionExtensions
{
    public static Type GetGenericType(this object o) => o.GetType().GetGenericType();

    public static Type GetGenericType(this Type t) => t.IsGenericType ? t.GetGenericTypeDefinition() : t;

    public static bool IsGenericType(this object o, Type type)
    {
        var t = o?.GetGenericType();
        while (t != null)
        {
            if (t == type) return true;
            t = t.BaseType?.GetGenericType();
        }
        return false;
    }
    
    public static Type[] GetGenericArguments(this object o, Type parent)
    {
        if (!o.IsGenericType(parent)) throw new ArgumentException($"Object is not of type {parent.Name}");
        var t = o.GetType();
        while (t.GetGenericType() != parent)
        {
            t = t!.BaseType;
        }
        return t!.GetGenericArguments();
    }
}