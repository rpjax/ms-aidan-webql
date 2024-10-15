namespace Webql.Parsing.Ast;

/// <summary>
/// Specifies the type of scope in a WebQL query.
/// </summary>
public enum WebqlScopeType
{
    /// <summary>
    /// Represents a scope for aggregation operations.
    /// </summary>
    Aggregation,

    /// <summary>
    /// Represents a scope for logical filtering operations.
    /// </summary>
    LogicalFiltering,

    /// <summary>
    /// Represents a scope for projection operations.
    /// </summary>
    Projection
}
