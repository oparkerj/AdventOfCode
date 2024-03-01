using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Parsing.Context;

/// <summary>
/// Parser lookup which will try to adapt the current input to the given type.
/// </summary>
public class TypeParse : IParserLookup<Type>
{
    public bool TryLookup(Type inputType, Type value, string extra, IReadOnlyParseContext context, out IParser parser)
    {
        if (ParseAdapt.TryAdapt(inputType, value, context, out var adapt))
        {
            parser = adapt ?? IdentityAdapter.Create(value);
            return true;
        }

        parser = default!;
        return false;
    }
}