using System.Numerics;
using AdventToolkit.New.Data;

namespace AdventToolkit.New.Interface;

/// <summary>
/// Represents a 3-dimensional area.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TNum"></typeparam>
public interface ICube<T, TNum> : IBound<T, Pos3<TNum>>
    where T : IBound<T, Pos3<TNum>>
    where TNum : INumber<TNum>
{
    /// <summary>
    /// X-axis length.
    /// </summary>
    TNum XLength { get; }

    /// <summary>
    /// Y-axis length.
    /// </summary>
    TNum YLength { get; }

    /// <summary>
    /// Z-axis length.
    /// </summary>
    TNum ZLength { get; }

    /// <summary>
    /// Minimum value on the X-axis.
    /// </summary>
    TNum MinX { get; }
    
    /// <summary>
    /// Minimum value on the Y-axis.
    /// </summary>
    TNum MinY { get; }
    
    /// <summary>
    /// Minimum value on the Z-axis.
    /// </summary>
    TNum MinZ { get; }
    
    /// <summary>
    /// Maximum value on the X-axis (inclusive).
    /// </summary>
    TNum MaxX { get; }
    
    /// <summary>
    /// Maximum value on the Y-axis (inclusive).
    /// </summary>
    TNum MaxY { get; }
    
    /// <summary>
    /// Maximum value on the Z-axis (inclusive).
    /// </summary>
    TNum MaxZ { get; }
    
    /// <summary>
    /// Ending value on the X-axis (exclusive).
    /// </summary>
    TNum EndX { get; }
    
    /// <summary>
    /// Ending value on the Y-axis (exclusive).
    /// </summary>
    TNum EndY { get; }
    
    /// <summary>
    /// Ending value on the Z-axis (exclusive).
    /// </summary>
    TNum EndZ { get; }
}