using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

/// <summary>
/// Provides methods to translate collection aggregation expressions.
/// </summary>
public static class CollectionAggregationExpressionTranslator
{
    /// <summary>
    /// Translates a collection aggregation expression.
    /// </summary>
    /// <param name="node">The node representing the collection aggregation expression.</param>
    /// <returns>The translated expression.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid operator is encountered.</exception>
    public static Expression TranslateCollectionAggregationExpression(WebqlOperationExpression node)
    {
        switch (node.GetCollectionAggregationOperator())
        {
            case WebqlCollectionAggregationOperator.Count:
                return TranslateCountExpression(node);

            case WebqlCollectionAggregationOperator.Contains:
                return TranslateContainsExpression(node);

            case WebqlCollectionAggregationOperator.Index:
                return TranslateIndexExpression(node);

            case WebqlCollectionAggregationOperator.Any:
                return TranslateAnyExpression(node);

            case WebqlCollectionAggregationOperator.All:
                return TranslateAllExpression(node);

            case WebqlCollectionAggregationOperator.Min:
                return TranslateMinExpression(node);

            case WebqlCollectionAggregationOperator.Max:
                return TranslateMaxExpression(node);

            case WebqlCollectionAggregationOperator.Sum:
                return TranslateSumExpression(node);

            case WebqlCollectionAggregationOperator.Average:
                return TranslateAverageExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    /// <summary>
    /// Translates a count expression.
    /// </summary>
    /// <param name="node">The node representing the count expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateCountExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetCountMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates a contains expression.
    /// </summary>
    /// <param name="node">The node representing the contains expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateContainsExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetContainsMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates an index expression.
    /// </summary>
    /// <param name="node">The node representing the index expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateIndexExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetIndexMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates an any expression.
    /// </summary>
    /// <param name="node">The node representing the any expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateAnyExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAnyMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates an all expression.
    /// </summary>
    /// <param name="node">The node representing the all expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateAllExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAllMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates a min expression.
    /// </summary>
    /// <param name="node">The node representing the min expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateMinExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetMinMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates a max expression.
    /// </summary>
    /// <param name="node">The node representing the max expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateMaxExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetMaxMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates a sum expression.
    /// </summary>
    /// <param name="node">The node representing the sum expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateSumExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetSumMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }

    /// <summary>
    /// Translates an average expression.
    /// </summary>
    /// <param name="node">The node representing the average expression.</param>
    /// <returns>The translated expression.</returns>
    public static Expression TranslateAverageExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        var methodInfo = context.LinqProvider.GetAverageMethodInfo(source: lhs);

        return Expression.Call(methodInfo, lhsExpression, rhsExpression);
    }
}
