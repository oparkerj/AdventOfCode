using AdventToolkit2.Parsing.Interface;

namespace AdventToolkit2.Parsing.Context;

/// <summary>
/// Allow an IParser to be given literally in the parse builder.
/// </summary>
public class LiteralParser : IParserLookup<IParser>
{
    public bool TryLookup(Type inputType, IParser value, string extra, IParseContext context, out IParser parser)
    {
        var parserInput = ParseUtil.GetParserTypesOf(value).InputType;

        if (ParseAdapt.TryAdapt(inputType, parserInput, context, out var adapt))
        {
            parser = adapt ?? value;
            return true;
        }

        parser = default!;
        return false;
    }
}