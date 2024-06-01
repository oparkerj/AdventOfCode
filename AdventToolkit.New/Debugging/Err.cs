using System.Diagnostics.CodeAnalysis;

namespace AdventToolkit.New.Debugging;

/// <summary>
/// This class only contains error-generating methods.
/// It serves the same purpose in this library as ThrowHelper
/// in C# source code.
/// </summary>
public static class Err
{
    [DoesNotReturn]
    public static void NotSupported()
    {
        throw new NotSupportedException();
    }
    
    [DoesNotReturn]
    public static void InvalidFormat()
    {
        throw new FormatException("Invalid input format.");
    }
}