namespace AdventToolkit.New.Transform;

public readonly struct StringToTypeTransformer<T> : ITransformSource<string, T>
{
    public readonly IStringTransform Source;
    public readonly ReadOnlySpanFunc<T> Func;

    public StringToTypeTransformer(IStringTransform source, ReadOnlySpanFunc<T> func)
    {
        Source = source;
        Func = func;
    }

    public T Result => Func(Source.Result);

    public T Apply(string input) => Func(input);
}