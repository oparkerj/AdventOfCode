namespace AdventToolkit.New.Transform;

public readonly struct StringTransformerSequence : IStringTransformSequence
{
    public readonly IStringTransform Source;
    public readonly ReadOnlySpanPartitionFunc PartitionFunc;

    public StringTransformerSequence(IStringTransform source, ReadOnlySpanPartitionFunc partitionFunc)
    {
        Source = source;
        PartitionFunc = partitionFunc;
    }

    public IEnumerable<string> Result
    {
        get
        {
            var result = new List<string>();
            PartitionFunc(Source.Result, span => result.Add(span.ToString()));
            return result;
        }
    }

    public IEnumerable<string> Apply(string input)
    {
        var result = new List<string>();
        PartitionFunc(Source.Apply(input), span => result.Add(span.ToString()));
        return result;
    }

    public void Apply(ReadOnlySpan<char> span, ReadOnlySpanAction action) => PartitionFunc(span, action);
}