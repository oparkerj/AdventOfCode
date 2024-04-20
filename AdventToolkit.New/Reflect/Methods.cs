using System.Linq.Expressions;
using System.Reflection;

namespace AdventToolkit.New.Reflect;

/// <summary>
/// Method reflection utilities.
/// </summary>
public static class Methods
{
    /// <summary>
    /// Get the method info from a method given in a lambda expression.
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static MethodInfo GetInfo(LambdaExpression expr)
    {
        return ((MethodCallExpression) expr.Body).Method;
    }

    /// <summary>
    /// Change or add generic types on a method info.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static MethodInfo WithGenericTypes(this MethodInfo info, params Type[] types)
    {
        return info.IsGenericMethodDefinition ? info.MakeGenericMethod(types) : info.GetGenericMethodDefinition().MakeGenericMethod(types);
    }
}