namespace AdventToolkit.New.Interface;

/// <summary>
/// Add operators for scaling. This is meant for compound types with multiple values.
/// Scaling the type typically means the scaling operation is applied to each individual
/// component in the type.
/// </summary>
/// <typeparam name="TSelf">Self type</typeparam>
/// <typeparam name="TOther">Scalar type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
public interface IScaleOperators<in TSelf, in TOther, out TResult>
    where TSelf : IScaleOperators<TSelf, TOther, TResult>
    where TOther : notnull
{
    /// <summary>
    /// Scale the value.
    /// </summary>
    /// <param name="left">Current value</param>
    /// <param name="right">Scalar</param>
    /// <returns>Scaled value</returns>
    static abstract TResult operator *(TSelf left, TOther right);
    
    /// <inheritdoc cref="op_Multiply"/>
    static virtual TResult operator checked *(TSelf left, TOther right) => left * right;
    
    /// <inheritdoc cref="op_Multiply"/>
    static abstract TResult operator /(TSelf left, TOther right);
    
    /// <inheritdoc cref="op_Multiply"/>
    static virtual TResult operator checked /(TSelf left, TOther right) => left / right;
}