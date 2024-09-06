using AdventToolkit2.Graphing.Interface;

namespace AdventToolkit2.Graphing;

public static class VertexExtensions
{
    /// <summary>
    /// Create and add a new vertex. As long as the vertex has a default constructor.
    /// </summary>
    /// <param name="model"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="THandle"></typeparam>
    /// <returns></returns>
    public static THandle Create<T, THandle>(this IVertexModel<T, THandle> model)
        where T : new() =>
        model.Add(new T());
}