using Webql.Core.Analysis;
using Webql.Parsing.Ast;

namespace Webql.Semantics.Extensions;

/// <summary>  
/// Provides extension methods for <see cref="WebqlOperationExpression"/> to analyze and categorize operators.  
/// </summary>  
public static class WebqlOperationExpressionSemanticExtensions
{
    /// <summary>  
    /// Gets the category of the operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The category of the operator.</returns>  
    public static WebqlOperatorCategory GetOperatorCategory(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetOperatorCategory(expression.Operator);
    }

    /// <summary>  
    /// Gets the semantic operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The semantic operator.</returns>  
    public static WebqlSemanticOperator GetSemanticOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetSemanticOperator(expression.Operator);
    }

    /// <summary>  
    /// Gets the collection operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The collection operator.</returns>  
    public static WebqlCollectionOperator GetCollectionOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionOperator(expression.Operator);
    }

    /// <summary>  
    /// Gets the collection manipulation operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The collection manipulation operator.</returns>  
    public static WebqlCollectionManipulationOperator GetCollectionManipulationOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionManipulationOperator(expression.Operator);
    }

    /// <summary>  
    /// Gets the collection aggregation operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The collection aggregation operator.</returns>  
    public static WebqlCollectionAggregationOperator GetCollectionAggregationOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionAggregationOperator(expression.Operator);
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is arithmetic.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is arithmetic; otherwise, <c>false</c>.</returns>  
    public static bool IsArithmetic(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Arithmetic;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is relational.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is relational; otherwise, <c>false</c>.</returns>  
    public static bool IsRelational(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Relational;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is string relational.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is string relational; otherwise, <c>false</c>.</returns>  
    public static bool IsStringRelational(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.StringRelational;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is logical.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is logical; otherwise, <c>false</c>.</returns>  
    public static bool IsLogical(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Logical;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is semantic.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is semantic; otherwise, <c>false</c>.</returns>  
    public static bool IsSemantic(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Semantic;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is a collection manipulation operator.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is a collection manipulation operator; otherwise, <c>false</c>.</returns>  
    public static bool IsCollectionManipulation(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.CollectionManipulation;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is a collection aggregation operator.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is a collection aggregation operator; otherwise, <c>false</c>.</returns>  
    public static bool IsCollectionAggregation(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.CollectionAggregation;
    }

    /// <summary>  
    /// Gets the arity of the operator in the given operation expression.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns>The arity of the operator.</returns>  
    public static WebqlOperatorArity GetOperatorArity(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetOperatorArity(expression.Operator);
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is unary.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is unary; otherwise, <c>false</c>.</returns>  
    public static bool IsUnary(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorArity() == WebqlOperatorArity.Unary;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is binary.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is binary; otherwise, <c>false</c>.</returns>  
    public static bool IsBinary(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorArity() == WebqlOperatorArity.Binary;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression requires operands to be of the same type.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator requires operands to be of the same type; otherwise, <c>false</c>.</returns>  
    public static bool OperatorRequiresOperandsToBeOfSameType(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.OperatorRequiresOperandsToBeOfSameType(expression.Operator);
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression represents a LINQ queryable method call operator.  
    /// </summary>  
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is a LINQ queryable method call operator; otherwise, <c>false</c>.</returns>  
    public static bool IsQueryableLinqOperator(this WebqlOperationExpression expression)
    {
        var operatorCategory = expression.GetOperatorCategory();

        return false
            || operatorCategory == WebqlOperatorCategory.CollectionManipulation
            || operatorCategory == WebqlOperatorCategory.CollectionAggregation;
    }

    /// <summary>  
    /// Determines whether the operator in the given operation expression is a collection operator.  
    /// </summary>  
    /// <remarks>
    /// A collection operator is an operator that manipulates or aggregates a collection.
    /// </remarks>
    /// <param name="expression">The operation expression.</param>  
    /// <returns><c>true</c> if the operator is a collection operator; otherwise, <c>false</c>.</returns>  
    public static bool IsCollectionOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.IsCollectionOperator(expression.Operator);
    }
}
