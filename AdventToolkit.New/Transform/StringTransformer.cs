namespace AdventToolkit.New.Transform;

public readonly struct StringTransformer : IStringTransform
{
    public readonly IStringTransform Source;
    public readonly ReadOnlySpanFunc Func;

    public StringTransformer(IStringTransform source, ReadOnlySpanFunc func)
    {
        Source = source;
        Func = func;
    }

    public string Result => Func(Source.Result).ToString();

    ReadOnlySpan<char> IStringTransform.Result => Func(Source.Result);

    public string Apply(string input) => Apply(input.AsSpan()).ToString();

    public ReadOnlySpan<char> Apply(ReadOnlySpan<char> input) => Func(Source.Apply(input));
}