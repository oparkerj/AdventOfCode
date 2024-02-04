namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// Represents a stage in the parse that modifies the object building the parse.
/// </summary>
public interface IModifier
{
    /// <summary>
    /// Try to apply the modification to the pipeline.
    /// </summary>
    /// <param name="pipeline">Current builder type.</param>
    /// <param name="inputType">Current parse input type.</param>
    /// <param name="value">Value given to the builder.</param>
    /// <param name="extra">Extra info given to the builder.</param>
    /// <param name="context">Parse context.</param>
    /// <typeparam name="TPipeline">Pipeline type.</typeparam>
    /// <typeparam name="T">Value type.</typeparam>
    /// <returns>True if the modifier was applied, false otherwise.</returns>
    bool TryApply<TPipeline, T>(TPipeline pipeline, Type inputType, T value, string extra, IReadOnlyParseContext context);
}

/// <summary>
/// A modifier made for a specific pipeline type.
/// </summary>
/// <typeparam name="TPipeline"></typeparam>
public interface IModifier<in TPipeline> : IModifier
{
    bool IModifier.TryApply<TPipe, T>(TPipe pipe, Type inputType, T value, string extra, IReadOnlyParseContext context)
    {
        if (pipe is TPipeline pipeline)
        {
            return TryApply(pipeline, inputType, value, extra, context);
        }
        return false;
    }

    /// <inheritdoc cref="IModifier.TryApply{TPipe, T}"/>
    bool TryApply<T>(TPipeline pipeline, Type inputType, T value, string extra, IReadOnlyParseContext context);
}

/// <summary>
/// A modifier made for a specific pipeline type and value type.
/// </summary>
/// <typeparam name="TPipeline"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface IModifier<in TPipeline, in TValue> : IModifier<TPipeline>
{
    bool IModifier<TPipeline>.TryApply<T>(TPipeline pipeline, Type inputType, T value, string extra, IReadOnlyParseContext context)
    {
        if (value is TValue v)
        {
            return TryApply<TValue>(pipeline, inputType, v, extra, context);
        }
        return false;
    }

    /// <inheritdoc cref="IModifier.TryApply{TPipe, T}"/>
    bool TryApply<T>(TPipeline pipeline, Type inputType, TValue value, string extra, IReadOnlyParseContext context);
}