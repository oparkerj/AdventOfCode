namespace AdventToolkit.New.Transform;

public readonly struct StringToTypeTransformerSequence<T> : ITransformSequence<string, T>
{
    public readonly IStringTransform Source;
    public readonly ReadOnlySpanPartitionFunc PartitionFunc;
    public readonly ReadOnlySpanFunc<T> Func;

    public StringToTypeTransformerSequence(IStringTransform source, ReadOnlySpanPartitionFunc partitionFunc, ReadOnlySpanFunc<T> func)
    {
        Source = source;
        PartitionFunc = partitionFunc;
        Func = func;
    }

    public IEnumerable<T> Result
    {
        get
        {
            var result = new List<T>();
            var func = Func;
            PartitionFunc(Source.Result, span => result.Add(func(span)));
            return result;
        }
    }

    public IEnumerable<T> Apply(string input)
    {
        var result = new List<T>();
        var func = Func;
        PartitionFunc(Source.Apply(input), span => result.Add(func(span)));
        return result;
    }
}