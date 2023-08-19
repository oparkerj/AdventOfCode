namespace AdventToolkit.New.Transform;

public interface IStringTransform : ITransformSource<string, string>
{
    new ReadOnlySpan<char> Result { get; }

    ReadOnlySpan<char> Apply(ReadOnlySpan<char> input);
}

public interface IStringTransformSequence : ITransformSequence<string, string> { }