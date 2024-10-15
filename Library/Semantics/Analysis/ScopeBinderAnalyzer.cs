using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Semantics.Scope;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a semantic context binder analyzer.
/// </summary>
/// <remarks>
/// This analyzer is responsible for creating and binding the appropriate scope to each syntax node.
/// </remarks>
public class ScopeBinderAnalyzer : SyntaxTreeAnalyzer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeBinderAnalyzer"/> class.
    /// </summary>
    public ScopeBinderAnalyzer()
    {

    }

    /// <summary>
    /// Analyzes the specified syntax node and binds the appropriate scope.
    /// </summary>
    /// <param name="node">The syntax node to analyze.</param>
    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                BindRootScope((WebqlQuery)node);
                break;

            case WebqlNodeType.Expression:
                BindExpressionScope((WebqlExpression)node);
                break;
        }

        base.Analyze(node);
    }

    /// <summary>
    /// Binds the root scope for the specified query node.
    /// </summary>
    /// <param name="node">The query node.</param>
    /// <exception cref="InvalidOperationException">Thrown when the query node is not the root node.</exception>
    private void BindRootScope(WebqlQuery node)
    {
        if (!node.IsRoot())
        {
            throw new InvalidOperationException("Invalid query node.");
        }

        var rootScope = new WebqlScope(
            parent: null
        );

        node.BindScope(rootScope);
    }

    /// <summary>
    /// Binds the scope for the specified expression node.
    /// </summary>
    /// <param name="node">The expression node.</param>
    private void BindExpressionScope(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Operation:
                BindOperationScope((WebqlOperationExpression)node);
                break;
        }
    }

    /// <summary>
    /// Binds the scope for the specified operation expression node.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    private void BindOperationScope(WebqlOperationExpression node)
    {
        var localScope = node.GetScope();

        if (!node.HasScopeAttribute())
        {
            localScope = localScope.CreateChildScope();
            node.BindScope(localScope);
        }

        foreach (var operand in node.Operands)
        {
            if (operand.HasScopeAttribute())
            {
                continue;
            }

            operand.BindScope(localScope.CreateChildScope());
        }
    }

}
