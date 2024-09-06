using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventToolkit2.Debugging;

/// <summary>
/// This class only contains error-generating methods.
/// It serves the same purpose in this library as ThrowHelper
/// in C# source code.
/// </summary>
public static class Err
{
    /// <summary>
    /// Placeholder for a branch which should be unreachable.
    /// </summary>
    /// <exception cref="UnreachableException"></exception>
    [DoesNotReturn]
    public static void Unreachable()
    {
        throw new UnreachableException();
    }
    
    /// <summary>
    /// Placeholder for a branch which should be unreachable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [DoesNotReturn]
    public static T Unreachable<T>()
    {
        Unreachable();
        return default;
    }
    
    /// <summary>
    /// Signals an operation that cannot be performed on the current type.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    [DoesNotReturn]
    public static void NotSupported()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Signals an invalid argument.
    /// </summary>
    /// <param name="msg"></param>
    /// <exception cref="ArgumentException"></exception>
    [DoesNotReturn]
    public static void Argument(string msg)
    {
        throw new ArgumentException(msg);
    }
    
    /// <summary>
    /// Signals an invalid argument.
    /// </summary>
    /// <param name="msg"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [DoesNotReturn]
    public static T Argument<T>(string msg)
    {
        Argument(msg);
        return default;
    }

    /// <summary>
    /// Signals an attempt to perform an action while in a bad state.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [DoesNotReturn]
    public static void InvalidOperation()
    {
        throw new InvalidOperationException();
    }
    
    /// <summary>
    /// Signals an attempt to perform an action while in a bad state.
    /// </summary>
    /// <param name="msg"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [DoesNotReturn]
    public static void InvalidOperation(string msg)
    {
        throw new InvalidOperationException(msg);
    }

    /// <summary>
    /// Signals an attempt to access data outside valid bounds.
    /// </summary>
    /// <exception cref="IndexOutOfRangeException"></exception>
    [DoesNotReturn]
    public static void OutOfBounds()
    {
        throw new IndexOutOfRangeException();
    }

    /// <summary>
    /// Signals that more elements were expected in a sequence.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    [DoesNotReturn]
    public static void EndOfSequence()
    {
        throw new InvalidOperationException();
    }
    
    /// <summary>
    /// Signals that the input is not valid.
    /// </summary>
    /// <exception cref="FormatException"></exception>
    [DoesNotReturn]
    public static void InvalidFormat()
    {
        throw new FormatException();
    }
    
    /// <summary>
    /// Signals that the input is not valid.
    /// </summary>
    /// <param name="msg"></param>
    /// <exception cref="FormatException"></exception>
    [DoesNotReturn]
    public static void InvalidFormat(string msg)
    {
        throw new FormatException(msg);
    }
}