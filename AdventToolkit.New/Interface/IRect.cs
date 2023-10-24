using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Interface;

/// <summary>
/// Represents a 2-dimensional area.
/// </summary>
/// <typeparam name="T">Rect type.</typeparam>
/// <typeparam name="TNum">Number type.</typeparam>
public interface IRect<T, TNum> : IBound<T, Pos<TNum>>
    where T : IRect<T, TNum>
    where TNum : INumber<TNum>
{
    /// <summary>
    /// Rect width.
    /// </summary>
    TNum Width { get; }
    
    /// <summary>
    /// Rect height.
    /// </summary>
    TNum Height { get; }
    
    /// <summary>
    /// Minimum value on the X-axis.
    /// </summary>
    TNum MinX { get; }
    
    /// <summary>
    /// Minimum value on the Y-axis.
    /// </summary>
    TNum MinY { get; }
    
    /// <summary>
    /// Maximum value on the X-axis (inclusive).
    /// </summary>
    TNum MaxX { get; }
    
    /// <summary>
    /// Maximum value on the Y-axis (inclusive).
    /// </summary>
    TNum MaxY { get; }
    
    /// <summary>
    /// Ending value on the X-axis (exclusive).
    /// </summary>
    TNum EndX { get; }
    
    /// <summary>
    /// Ending value on the Y-axis (exclusive).
    /// </summary>
    TNum EndY { get; }
}