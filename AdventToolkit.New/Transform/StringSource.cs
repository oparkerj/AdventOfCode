namespace AdventToolkit.New.Transform;

public readonly record struct StringSource(string Source) : IStringTransform
{
    public string Result => Source;
    
    ReadOnlySpan<char> IStringTransform.Result => Source;

    public string Apply(string input) => input;
    
    public ReadOnlySpan<char> Apply(ReadOnlySpan<char> input) => input;
}