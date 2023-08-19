using AdventToolkit.New.Extensions;

namespace AdventToolkit.New.Transform;

public static class StringTransformerExtensions
{
    public static StringSource Parse(this string s) => new(s);

    public static StringToTypeTransformer<T> As<T>(this IStringTransform transform)
        where T : ISpanParsable<T>
    {
        return new StringToTypeTransformer<T>(transform, static span => T.Parse(span, null));
    }

    public static StringToTypeTransformerSequence<T> As<T>(this StringTransformerSequence sequence)
        where T : ISpanParsable<T>
    {
        return new StringToTypeTransformerSequence<T>(
            sequence.Source,
            sequence.PartitionFunc,
            static span => T.Parse(span, null));
    }
    
    public static StringTransformerSequence EachLine(this IStringTransform transform)
    {
        return new StringTransformerSequence(transform, static (span, action) =>
        {
            foreach (var line in span.EnumerateLines())
            {
                action(line);
            }
        });
    }

    public static StringTransformerSequence Split(this IStringTransform transform, char c)
    {
        return new StringTransformerSequence(transform, (span, action) =>
        {
            foreach (var part in span.EnumerateSplit(c))
            {
                action(part);
            }
        });
    }
}