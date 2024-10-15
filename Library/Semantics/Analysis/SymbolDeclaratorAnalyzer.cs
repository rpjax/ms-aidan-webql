using Webql.Core.Extensions;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

/// <summary>  
/// Represents a visitor for declaring symbol declarations in the Webql syntax tree.  
/// </summary>  
public class SymbolDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    /// <summary>  
    /// Initializes a new instance of the <see cref="SymbolDeclaratorAnalyzer"/> class.  
    /// </summary>  
    public SymbolDeclaratorAnalyzer()
    {
    }

    /// <inheritdoc/>  
    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                DeclareQuerySymbols((WebqlQuery)node);
                break;

            case WebqlNodeType.Expression:
                DeclareExpressionSymbols((WebqlExpression)node);
                break;
        }

        base.Analyze(node);
    }

    /// <summary>  
    /// Declares the symbols for the query node.  
    /// </summary>  
    /// <param name="node">The query node.</param>  
    /// <exception cref="InvalidOperationException">Thrown when the query node is not the root node.</exception>  
    private void DeclareQuerySymbols(WebqlQuery node)
    {
        if (node.IsNotRoot())
        {
            throw new InvalidOperationException("Query node must be the root node.");
        }

        var queryableType = node.GetQueryableType();
        var elementType = node.GetCompilationElementType();

        var sourceType = queryableType.MakeGenericType(elementType);

        node.DeclareSourceSymbol(sourceType);
    }

    /// <summary>  
    /// Declares the symbols for the expression node.  
    /// </summary>  
    /// <param name="node">The expression node.</param>  
    private void DeclareExpressionSymbols(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Operation:
                DeclareOperationExpressionSymbols((WebqlOperationExpression)node);
                break;
        }
    }

    /// <summary>  
    /// Declares the symbols for the operation expression node.  
    /// </summary>  
    /// <param name="node">The operation expression node.</param>  
    private void DeclareOperationExpressionSymbols(WebqlOperationExpression node)
    {
        if (!node.IsCollectionOperator())
        {
            return;
        }

        node.EnsureAtLeastOneOperand();

        var lhsExpression = node.Operands[0];

        lhsExpression.EnsureIsQueryable();

        var lhsSemantics = lhsExpression.GetExpressionSemantics();
        var lhsType = lhsSemantics.Type;
        var elementType = lhsType.GetQueryableElementType();

        /*  
         * Operands that are of a queryable type do not need to access the symbol declaration.  
         * This avoids conflicts, for example:  
         *   
         * $filter: [element.someList, (element) => {  }]   
         *   
         * Since the filter operator is a collection manipulation operator, its node declares an 'element' symbol.  
         * When the left operand (element.someList) is resolved, it should look for the 'element' symbol in the parent scope.  
         * 
         * NOTE: well, it seems i forgot that semantic analysis is not available at this stage, since the symbols are not declared yet.
         * So there is no way to know if the operand is a queryable type or not. However, the AST is built in a way where
         * the first operand is always the source queryable, so we can safely skip the first operand.
         */
        
        foreach (var operand in node.Operands.Skip(1))
        {
            //var skipOperand = true
            //    && operand.GetExpressionType().IsQueryable();

            //if (skipOperand)
            //{
            //    continue;
            //}

            operand.DeclareElementSymbol(elementType);
        }
    }
}
