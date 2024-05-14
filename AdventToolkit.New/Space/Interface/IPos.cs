using System.Numerics;
using AdventToolkit.New.Interface;
using AdventToolkit.New.Parsing.Interface;

namespace AdventToolkit.New.Space.Interface;

/// <summary>
/// Represents an N-dimensional position type.
/// </summary>
/// <typeparam name="T">Type of the position.</typeparam>
/// <typeparam name="TNum">Component type of the position.</typeparam>
public interface IPos<T, TNum> :
    IAdditionOperators<T, T, T>,
    ISubtractionOperators<T, T, T>,
    IMultiplyOperators<T, T, T>,
    IScaleOperators<T, TNum, T>,
    IDivisionOperators<T, T, T>,
    IUnaryNegationOperators<T, T>,
    IComparisonOperators<T, T, bool>,
    IAdditiveIdentity<T, T>,
    IDecrementOperators<T>,
    IIncrementOperators<T>,
    ISpanParsable<T>,
    IDimension<T>,
    ITypeDescriptor
    where T : IPos<T, TNum>
    where TNum : notnull
{
    /// <summary>
    /// Value where all components are zero. This represents the origin for
    /// the position type.
    /// </summary>
    static abstract T Zero { get; }
    
    /// <summary>
    /// Value where all components are one.
    /// </summary>
    static abstract T One { get; }

    /// <summary>
    /// Parse a position from a simplified span.
    /// This span contains only the components of the position joined by the given split
    /// character, and optionally whitespace.
    /// </summary>
    /// <param name="span">Input span.</param>
    /// <param name="separator">Component separator.</param>
    /// <returns>Parsed position.</returns>
    static abstract T ParseSimple(ReadOnlySpan<char> span, char separator = ',');

    /// <summary>
    /// Compute the distance to another position.
    /// The distance is computed using taxicab or manhattan geometry.
    /// </summary>
    /// <param name="other">Other position</param>
    /// <returns>Manhattan distance</returns>
    TNum Dist(T other);

    /// <summary>
    /// Get the minimum component of the position.
    /// </summary>
    /// <returns>Minimum component</returns>
    TNum Min();

    /// <summary>
    /// Get the maximum component of the position.
    /// </summary>
    /// <returns>Maximum component</returns>
    TNum Max();

    /// <summary>
    /// Get a component-wise minimum between this and another position.
    /// </summary>
    /// <param name="other">Other position</param>
    /// <returns>Component-wise minimum position</returns>
    T Min(T other);

    /// <summary>
    /// Get a component-wise maximum between this and another position.
    /// </summary>
    /// <param name="other">Other position</param>
    /// <returns>Component-wise maximum position</returns>
    T Max(T other);

    /// <summary>
    /// Get a position with each component normalized.
    /// This effectively means passing each component to <see cref="Math.Sign(int)"/>
    /// </summary>
    /// <returns></returns>
    T Normalize();

    /// <summary>
    /// Returns the sum of all components.
    /// </summary>
    /// <returns></returns>
    TNum Sum();

    /// <summary>
    /// Get adjacent positions.
    /// Adjacent positions share one full "side" with another position.
    /// This is typically positions with a Manhattan distance of 1.
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> Adjacent();

    /// <summary>
    /// Get positions around.
    /// This is any position which touches the original position.
    /// This is typically adjacent positions plus diagonals.
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> Around();
}