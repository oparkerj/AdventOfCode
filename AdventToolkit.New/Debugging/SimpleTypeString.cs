using System.Runtime.CompilerServices;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Debugging;

/// <summary>
/// Behaves like the default string interpolation handler but
/// <see cref="Type"/> interpolations are passed to <see cref="Types.SimpleName"/>.
/// </summary>
[InterpolatedStringHandler]
public ref struct SimpleTypeString
{
    private DefaultInterpolatedStringHandler _handler;

    private SimpleTypeString(DefaultInterpolatedStringHandler handler) => _handler = handler;
        
    public SimpleTypeString(int literalCount, int formattedCount) => _handler = new DefaultInterpolatedStringHandler(literalCount, formattedCount);

    /// <summary>
    /// Implicit conversion of string to <see cref="SimpleTypeString"/>
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static implicit operator SimpleTypeString(string s)
    {
        var handler = new DefaultInterpolatedStringHandler(s.Length, 0);
        handler.AppendLiteral(s);
        return new SimpleTypeString(handler);
    }

    public void AppendLiteral(string s) => _handler.AppendLiteral(s);
        
    public void AppendFormatted(Type? type) => _handler.AppendFormatted(type?.SimpleName() ?? "null");

    public void AppendFormatted<T>(T t) => _handler.AppendFormatted(t);

    public override string ToString() => _handler.ToString();

    public string ToStringAndDispose() => _handler.ToStringAndClear();
}