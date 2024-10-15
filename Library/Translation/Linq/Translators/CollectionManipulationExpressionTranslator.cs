using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

/// <summary>
/// Provides methods to translate collection manipulation expressions.
/// </summary>
public static class CollectionManipulationExpressionTranslator
{
    /// <summary>
    /// Translates a collection manipulation expression.
    /// </summary>
    /// <param name="node">The node representing the collection manipulation expression.</param>
    /// <returns>The translated expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid operator is encountered.</exception>
    public static Expression TranslateCollectionManipulationExpression(WebqlOperationExpression node)
    {
        switch (node.GetCollectionManipulationOperator())
        {
            case WebqlCollectionManipulationOperator.Filter:
                return TranslateFilterExpression(node);

            case WebqlCollectionManipulationOperator.Select:
                return TranslateSelectExpression(node);

            case WebqlCollectionManipulationOperator.SelectMany:
                return TranslateSelectManyExpression(node);

            case WebqlCollectionManipulationOperator.Limit:
                return TranslateLimitExpression(node);

            case WebqlCollectionManipulationOperator.Skip:
                return TranslateSkipExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    /// <summary>
    /// Translates a filter expression.
    /// </summary>
    /// <param name="node">The node representing the filter expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateFilterExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var elementParameter = rhs.GetElementParameterExpression();
        var lambdaExpression = Expression.Lambda(rhsExpression, elementParameter);

        var methodInfo = node.GetLinqProvider()
            .GetWhereMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, lambdaExpression);
    }

    /// <summary>
    /// Translates a select expression.
    /// </summary>
    /// <param name="node">The node representing the select expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateSelectExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var elementParameter = rhs.GetElementParameterExpression();
        var lambdaExpression = Expression.Lambda(rhsExpression, elementParameter);

        var methodInfo = node.GetLinqProvider()
            .GetSelectMethodInfo(source: lhs, selector: rhs);

        return Expression.Call(methodInfo, lhsExpression, lambdaExpression);
    }

    /// <summary>
    /// Translates a select many expression.
    /// </summary>
    /// <param name="node">The node representing the select many expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateSelectManyExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Translates a limit expression.
    /// </summary>
    /// <param name="node">The node representing the limit expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateLimitExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = node.GetLinqProvider()
            .GetTakeMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates a skip expression.
    /// </summary>
    /// <param name="node">The node representing the skip expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateSkipExpression(WebqlOperationExpression node)
    {
        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = node.GetLinqProvider()
            .GetSkipMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }
}
