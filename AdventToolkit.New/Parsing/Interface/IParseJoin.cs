namespace AdventToolkit.New.Parsing.Interface;

/// <summary>
/// A parser with the ability to join another parser to its inner components.
/// </summary>
public interface IParseJoin
{
    /// <summary>
    /// Perform an inner join to this component.
    /// </summary>
    /// <param name="parser">Join parser.</param>
    /// <param name="targetLevel">Level at which to insert the parser.
    /// When 0, the parser should be joined to the current component. When greater
    /// than 0, the parser should be inner-joined to further nested components.</param>
    /// <param name="context">Parse context.</param>
    /// <returns>Joined parser.</returns>
    /// TODO See ParseJoin
    IParser InnerJoin(IParser parser, int targetLevel, IParseContext context);
}